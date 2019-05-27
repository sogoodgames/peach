using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatApp : App
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public GameObject ChatSelectionScreen;
    public GameObject ChatScreen;

    // chat selection screen
    public Transform ChatButtonsParent;
    public GameObject ChatButtonPrefab;

    // chat messages screen
    public Text FriendTitleText;
    public Transform ChatBubblesParent;
    public Transform ChatOptionsParent;
    public GameObject PlayerChatBubblePrefab;
    public GameObject FriendChatBubblePrefab;
    public GameObject MessageOptionPrefab;
    public ScrollRect ChatBubblesScrollRect;
    public ChatAttachment ChatAttachment;

    //Audio
    public AudioSource typingSFX;
    public AudioSource messageSFX;

    //Message timing
    public float SkipCooldown = 0.5f;
    public float MaxTimeBetweenMessages = 2f;
    private float timeSinceLastMessage = 0f;

    // internal
    private Chat m_activeChat;
    private bool m_needsScroll;
    private int scrollWait = 0;

    private IEnumerator m_RunMessagesCoroutine;
    private IEnumerator m_drawBubblesCoroutine;

    // ------------------------------------------------------------------------
    // Methods : MonoBehaviour
    // ------------------------------------------------------------------------
    // why is 3 the magic number of frames to get an accurate scroll position?
    // i have no idea.
    // why am i not using a more elegant solution using ienumerators?
    // because i'm tired.
    protected override void Update() {
        base.Update();
        if(animate) {
            return;
        }

        if(m_needsScroll) {
            scrollWait++;
            if(scrollWait >= 3) {
                ChatBubblesScrollRect.normalizedPosition = new Vector2(0, 0);
                m_needsScroll = false;
                scrollWait = 0;
            }
        }
    }

    // ------------------------------------------------------------------------
    // Methods : App
    // ------------------------------------------------------------------------
    public override void Open() {
        base.Open();
        PhoneOS.ReturnButton.SetActive(true);
        // always reset to chat selection bc i'm lazy lol
        OpenChatSelection();
    }

    // ------------------------------------------------------------------------
    public override void OnCloseAnimationFinished () {
        m_activeChat = null;
        PhoneOS.ReturnButton.SetActive(false);
        base.OnCloseAnimationFinished();
        CloseChatSelection();
        CloseChat();
        ChatAttachment.Close();
    }

    // ------------------------------------------------------------------------
    public override void Return() {
        if(animate) {
            return;
        }
        
        if(ChatScreen.activeInHierarchy) {
            CloseChat();
            OpenChatSelection();
        } else if(m_activeChat != null) {
            CloseChatSelection();
            OpenChat(m_activeChat);
        } else {
            Close();
        }
    }

    // ------------------------------------------------------------------------
    // Methods : Public
    // ------------------------------------------------------------------------
    public void OpenChatSelection () {
        CloseChat();

        // populate list of chat buttons
        // we do this every time we open the app in case it's changed
        foreach(Chat chat in PhoneOS.ActiveChats) {
            GameObject chatButtonObj = Instantiate(
                ChatButtonPrefab,
                ChatButtonsParent
            ) as GameObject;

            OpenChatButton chatButton = chatButtonObj.GetComponent<OpenChatButton>();
            if(chatButton) {
                // set name
                chatButton.NameText.text = chat.Friend.ToString();

                // set profile pic
                chatButton.ProfilePic.sprite = PhoneOS.UserIconAssets[chat.Icon];

                // show unread notif (if unfinished)
                if(chat.finished) {
                    chatButton.UnreadNotif.SetActive(false);
                } else {
                    chatButton.UnreadNotif.SetActive(true);
                }

                // bind button to open this chat
                chatButton.OpenButton.onClick.AddListener(
                    delegate {OpenChat(chat);}
                );

            } else {
                Debug.LogError("Chat Button Prefab does not contain an OpenChatButton.");
            }
        }

        ChatSelectionScreen.SetActive(true);
    }

    // ------------------------------------------------------------------------
    public void OpenChat (Chat c) {
        if(c == null) {
            Debug.LogError("Trying to open chat with null chat");
            return;
        }
        if(animate) {
            return;
        }

        CloseChatSelection();
        m_activeChat = c;

        Debug.Log("opening chat: " + c.Friend + "; last visited message: " + m_activeChat.GetLastVisitedMessage().Node);

        // set name
        FriendTitleText.text = c.Friend.ToString();

        // draw chat bubbles for all of the messages we've read so far
        FillChatWithVisitedMessages();

        ChatScreen.SetActive(true);
        MoveConversation();
    }

    // ------------------------------------------------------------------------
    // Methods : Conversation running
    // ------------------------------------------------------------------------
    // i know this creates some duplicated code,
    // but avoiding the coroutines is a lot more convienent 
    // for just wanting to draw all of the read messages with no delay
    private void FillChatWithVisitedMessages () { 
        foreach(Message message in m_activeChat.VisitedMessages) {
            // record any clues found
            // in case the player missed them last time
            if(message.ClueGiven != ClueID.NoClue) {
                PhoneOS.FoundClue(message.ClueGiven);
            }

            // draw the message
            if(message.Player) {
                if(message.HasOptions()) {
                    if(message.MadeSelection()) {
                        DrawChatBubble(message, 0, PlayerChatBubblePrefab);
                    } else {
                        DrawChatOptions(message);
                    }
                } else {
                    for (int i = 0; i < message.Messages.Length; i++) {
                        DrawChatBubble(message, i, PlayerChatBubblePrefab);
                    }
                }
            } else {
                for (int i = 0; i < message.Messages.Length; i++) {
                    DrawChatBubble(message, i, FriendChatBubblePrefab);
                }
            }
        }
        m_needsScroll = true;
    }

    // ------------------------------------------------------------------------
    private void MoveConversation () {
        if(m_activeChat == null) {
            Debug.Log("Trying to move in conversation that hasn't been set.");
            return;
        }

        if(m_activeChat.finished) {
            return;
        }

        Message lastMessage = m_activeChat.GetLastVisitedMessage();
        int nextNode = -1;

        // find next message in convo
        if(lastMessage.HasOptions()) {
            if(lastMessage.MadeSelection()) {
                // if we made a selection, move to the next message
                nextNode = lastMessage.Branch[lastMessage.OptionSelection];
            } else {
                // if we have an unchosen option, don't do anything
                return;
            }
        } else {
            nextNode = lastMessage.Branch[0];
        }

        // draw the next message
        Message nextMessage = m_activeChat.GetMessage(nextNode);
        if(nextMessage == null){
            FinishConvo();
            return;
        }
        m_RunMessagesCoroutine = RunMessage(nextMessage);
        StartCoroutine(m_RunMessagesCoroutine);
    }

    // ------------------------------------------------------------------------
    private void FinishConvo () {
        Debug.Log("Reached end of convo at node " + m_activeChat.GetLastVisitedMessage().Node);
        m_activeChat.finished = true;

        // these should be events
        if(m_activeChat.Friend == Friend.Jin) {
            ChatAttachment.Open(PhoneOS.JinEndingPhoto/*, 1080, 810*/);
        } else if(m_activeChat.Friend == Friend.Emma && m_activeChat.GetLastVisitedMessage().Node == 16) {
            ChatAttachment.Open(PhoneOS.EmmaEndingPhoto/*, 1078, 1437*/);
        }
    }

    // ------------------------------------------------------------------------
    // Methods : Drawing UI
    // ------------------------------------------------------------------------
    private void DrawChatBubble (Message message, int messageIndex, GameObject prefab) {
        // create bubble object
        GameObject bubble = Instantiate(
            prefab,
            ChatBubblesParent
        ) as GameObject;

        timeSinceLastMessage = 0f;

        // find ChatBubbleUI
        ChatBubbleUI chatBubbleUi = bubble.GetComponent<ChatBubbleUI>();
        if(!chatBubbleUi) {
            return;
        }

        // fill text, icon, and image
        chatBubbleUi.Text.text = message.Messages[messageIndex];

        if(!message.Player) {
            chatBubbleUi.Icon.sprite = PhoneOS.UserIconAssets[m_activeChat.Icon];
        }

        if(messageIndex == message.Messages.Length - 1 && message.Image >= 0) {
            chatBubbleUi.Text.text = message.Messages[messageIndex] + " [click to open attachment]";
            chatBubbleUi.Button.onClick.AddListener(
                delegate {OpenAttachment(message);}
            );
            chatBubbleUi.Button.interactable = true;
        }
    }

    public void OpenAttachment (Message message) {
        if(animate) {
            return;
        }

        ChatAttachment.Open(PhoneOS.PhotoAssets[message.Image]/*, message.ImageWidth, message.ImageHeight*/);
    }

    // ------------------------------------------------------------------------
    private void DrawChatOptions (Message message) {
        if(message == null) {
            Debug.LogError("Message null.");
            return;
        }
        if(!message.Player) {
            Debug.LogError("Hecked up script config. NPC has chat options.");
            return;
        }
        if(!message.HasOptions()) {
            Debug.LogError("Attempting to draw options for message with no options.");
            return;
        }

        // if we've answered this question multiple times, mark this convo done
        if(m_activeChat.VisitedMessages.FindAll(m => m.Node == message.Node).Count > 1) {
            FinishConvo();
            return;
        }

        for(int i = 0; i < message.Options.Length; i++) {
            // if we've already been to this conversation option,
            // skip drawing whatever option we selected last time
            if(message.MadeSelection() && i == message.OptionSelection) {
                continue;
            }

            // draw bubble
            GameObject option = Instantiate(
                MessageOptionPrefab,
                ChatOptionsParent
            ) as GameObject;

            // setup bubble
            MessageButton messageButton = option.GetComponent<MessageButton>();
            
            // check if clue needs met
            if(PhoneOS.ClueRequirementMet(message.ClueNeeded[i])) {
                // set button text & hook up option function
                messageButton.Text.text = message.Options[i];
                SetButtonListener(messageButton.Button, message, i);
                //Debug.Log("created option [" + message.options[i] + "] with index " + i + " for message " + message.node);
            } else {
                // mark it as unavilable
                messageButton.Text.text = "[clue needed]";
            }
        }
    }

    // ------------------------------------------------------------------------
    // Methods : Conversation coroutines
    // ------------------------------------------------------------------------
    private IEnumerator RunMessage (Message message) {
        if(message == null) {
            Debug.LogError("Message null.");
            yield break;
        }

        // record that we visited this message (don't force)
        m_activeChat.VisitMessage(message, false);

        // draw either player or friend messages
        if(message.Player) {
            // if this has options, draw them; otherwise, draw messages
            if(message.HasOptions()) {
                DrawChatOptions(message);
            } else {
                m_drawBubblesCoroutine = RunChatBubbles(message, PlayerChatBubblePrefab);
                yield return StartCoroutine(m_drawBubblesCoroutine);
            }
        } else {
            m_drawBubblesCoroutine = RunChatBubbles(message, FriendChatBubblePrefab);
            yield return StartCoroutine(m_drawBubblesCoroutine);
        }

        // record any clues found
        if(message.ClueGiven != ClueID.NoClue) {
            PhoneOS.FoundClue(message.ClueGiven);
        }

        // if we're not waiting on an option selection, draw the next message
        if(!message.HasOptions()) {
            MoveConversation();
        }
    }

    // ------------------------------------------------------------------------
    private IEnumerator RunChatBubbles (Message message, GameObject prefab) {
        if(message == null) {
            Debug.LogError("Message null.");
            yield break;
        }

        // iterate over all of the messages in this node
        for (int i = 0; i < message.Messages.Length; i++) {
            float t = 2;
            if((message.Node == 0 && i == 0) || message.HasOptions()) {
                t = 0;
            }
            yield return new WaitForSeconds(t);
            //Debug.Log("drawing line: " + i);

            messageSFX.Play();
            DrawChatBubble(message, i, prefab);
            m_needsScroll = true;
        }
    }

    // ------------------------------------------------------------------------
    // Methods : Buttons
    // ------------------------------------------------------------------------
    private void SetButtonListener(Button b, Message m, int i) {
        b.onClick.AddListener(
            delegate {SelectOption(m, i);}
        );
    }

    // ------------------------------------------------------------------------
    private void SelectOption (Message message, int option) {
        typingSFX.Play();

        if (animate) {
            return;
        }
        
        if(message == null) {
            Debug.LogError("Message null.");
            return;
        }

        //Debug.Log("selected option " + option + " for message " + message.Node);

        // record in message that this option has been chosen
        message.OptionSelection = option;
        message.Messages = new string[1];
        message.Messages[0] = message.Options[option];

        // draw chosen message
        m_drawBubblesCoroutine = RunChatBubbles(message, PlayerChatBubblePrefab);
        StartCoroutine(m_drawBubblesCoroutine);

        // destroy option bubbles
        foreach(Transform child in ChatOptionsParent.transform) {
            Destroy(child.gameObject);
        }

        // force record that we visited this message
        m_activeChat.VisitMessage(message, true);

        // draw next chat
        MoveConversation();
    }

    // ------------------------------------------------------------------------
    // Methods : Closing UI
    // ------------------------------------------------------------------------
    private void CloseChat () {
        if(m_RunMessagesCoroutine != null) {
            StopCoroutine(m_RunMessagesCoroutine);
        }
        if(m_drawBubblesCoroutine != null) {
            StopCoroutine(m_drawBubblesCoroutine);
        }

        foreach(Transform child in ChatBubblesParent.transform) {
            Destroy(child.gameObject);
        }
        foreach(Transform child in ChatOptionsParent.transform) {
            Destroy(child.gameObject);
        }
        ChatScreen.SetActive(false);
    }

    // ------------------------------------------------------------------------
    private void CloseChatSelection () {
        foreach(Transform child in ChatButtonsParent.transform) {
            Destroy(child.gameObject);
        }
        ChatSelectionScreen.SetActive(false);
    }
}
