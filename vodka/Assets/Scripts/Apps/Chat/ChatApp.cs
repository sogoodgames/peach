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
    void Update() {
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
        // always reset to chat selection bc i'm lazy lol
        OpenChatSelection();
    }

    // ------------------------------------------------------------------------
    public override void Close () {
        base.Close();
        CloseChatSelection();
        CloseChat();
    }

    // ------------------------------------------------------------------------
    public override void Return() {
        if(ChatScreen.activeInHierarchy) {
            CloseChat();
            OpenChatSelection();
        } else {
            PhoneOS.GoHome();
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
                // ??

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
        CloseChatSelection();
        m_activeChat = c;

        Debug.Log("opening chat: " + c.Friend + "; active node: " + c.lastNode);

        // draw chat bubbles for all of the messages we've read so far
        FillChatWithVisitedMessages();

        ChatScreen.SetActive(true);
        MoveConversation();
    }

    // ------------------------------------------------------------------------
    // Methods : Conversation running
    // ------------------------------------------------------------------------
    // i know this creates a lot of duplicated code,
    // but avoiding the coroutines is a lot more convienent 
    // for just wanting to draw all of the read messages with no delay
    private void FillChatWithVisitedMessages () { 
        foreach(Message message in m_activeChat.visitedMessages) {
            if(message.Player) {
                if(message.HasOptions()) {
                    if(message.MadeSelection()) {
                        DrawChatBubble(message.Messages[0], PlayerChatBubblePrefab);
                    } else {
                        DrawChatOptions(message);
                    }
                } else {
                    foreach(string text in message.Messages) {
                        DrawChatBubble(text, PlayerChatBubblePrefab);
                    }
                }
            } else {
                foreach(string text in message.Messages) {
                    DrawChatBubble(text, FriendChatBubblePrefab);
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

        Message lastMessage = m_activeChat.GetMessage(m_activeChat.lastNode);

        // increment lastNode
        if(lastMessage.HasOptions()) {
            if(lastMessage.MadeSelection()) {
                // if we made a selection, move to the next message
                m_activeChat.lastNode = lastMessage.Branch[lastMessage.OptionSelection];
            } else {
                // if we have an unchosen option, don't do anything
                return;
            }
        } else {
            m_activeChat.lastNode = lastMessage.Branch[0];
        }

        // draw the next message
        Message nextMessage = m_activeChat.GetMessage(m_activeChat.lastNode);
        if(nextMessage == null){
            Debug.Log("Reached end of convo at node " + m_activeChat.lastNode);
            m_activeChat.finished = true;
            return;
        }
        m_RunMessagesCoroutine = RunMessage(nextMessage);
        StartCoroutine(m_RunMessagesCoroutine);
    }

    // ------------------------------------------------------------------------
    // Methods : Drawing UI
    // ------------------------------------------------------------------------
    private void DrawChatBubble (string text, GameObject prefab) {
        // create bubble object
        GameObject bubble = Instantiate(
            prefab,
            ChatBubblesParent
        ) as GameObject;

        // fill text with message text
        Text textObj = bubble.GetComponentInChildren<Text>();
        if(textObj) {
            textObj.text = text;
        } else {
            Debug.LogError("No Text component found on chat bubble prefab: " + prefab);
        }
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
                SetButtonListener(messageButton.Button, i);
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

        // record that we drew this message
        if(!m_activeChat.visitedMessages.Contains(message)) {
            m_activeChat.visitedMessages.Add(message);
            //Debug.Log("added message: " + message.Node);
        }

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
            if(message.Player || (message.Node == 0 && i == 0)) {
                t = 0;
            }
            yield return new WaitForSeconds(t);
            //Debug.Log("drawing line: " + i);

            DrawChatBubble(message.Messages[i], prefab);

            m_needsScroll = true;
        }
    }

    // ------------------------------------------------------------------------
    // Methods : Buttons
    // ------------------------------------------------------------------------
    private void SetButtonListener(Button b, int i) {
        b.onClick.AddListener(
            delegate {SelectOption(i);}
        );
    }

    // ------------------------------------------------------------------------
    private void SelectOption (int option) {
        Message message = m_activeChat.GetMessage(m_activeChat.lastNode);
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
        m_activeChat = null;
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
