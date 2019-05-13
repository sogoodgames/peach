using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatApp : App
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public PhoneOS PhoneOS;
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

    // internal
    private Chat m_activeChat;

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
                chatButton.NameText.text = chat.friend.ToString();

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

        Debug.Log("opening chat: " + c.friend + "; active node: " + c.lastNode);

        // draw chat bubbles for all of the messages we've read so far
        foreach (Message message in c.visitedMessages) {
            DrawMessage(message);
        }

        // draw next chat
        MoveConversation();

        ChatScreen.SetActive(true);
    }

    // ------------------------------------------------------------------------
    // Methods : Private
    // ------------------------------------------------------------------------
    private void MoveConversation () {
        if(m_activeChat == null) {
            Debug.Log("Trying to move in conversation that hasn't been set.");
            return;
        }

        Message lastMessage = m_activeChat.GetMessage(m_activeChat.lastNode);

        // record the last message we saw
        if(!m_activeChat.visitedMessages.Contains(lastMessage)) {
            m_activeChat.visitedMessages.Add(lastMessage);
        }

        // increment lastNode
        if(lastMessage.HasOptions()) {
            if(lastMessage.optionChosen) {
                // if we made a selection, move to the next message
                m_activeChat.lastNode = lastMessage.branch[lastMessage.optionSelection];
            } else {
                // if we have an unchosen option, don't do anything
                return;
            }
        } else {
            m_activeChat.lastNode = lastMessage.branch[0];
        }

        // draw the next message
        Message nextMessage = m_activeChat.GetMessage(m_activeChat.lastNode);
        if(nextMessage == null){
            // mark convo as complete
            // ??
            Debug.Log("Reached end of convo at node " + m_activeChat.lastNode);

            return;
        }
        DrawMessage(nextMessage);
    }

    private void DrawMessage (Message message) {
        if(message == null) {
            Debug.LogError("Message null.");
            return;
        }

        //Debug.Log("drawing message: " + message.node);

        // draw either player or friend messages
        if(message.player) {
            // if this has options, draw them; otherwise, draw messages
            if(message.HasOptions()) {
                DrawChatOptions(message);
            } else {
                CreateChatBubbles(message, PlayerChatBubblePrefab);
            }
        } else {
            CreateChatBubbles(message, FriendChatBubblePrefab);
        }

        // if we're not waiting on an option selection, draw the next message
        if(!message.HasOptions()) {
            MoveConversation();
        }
    }

    // ------------------------------------------------------------------------
    private void CreateChatBubbles (Message message, GameObject prefab) {
        if(message == null) {
            Debug.LogError("Message null.");
            return;
        }

        // iterate over all of the messages in this node
        foreach(string messageText in message.messages) {
            // create bubble object
            GameObject bubble = Instantiate(
                prefab,
                ChatBubblesParent
            ) as GameObject;

            // fill text with message text
            Text text = bubble.GetComponentInChildren<Text>();
            if(text) {
                text.text = messageText;
            } else {
                Debug.LogError("No Text component found on chat bubble prefab: " + prefab);
            }
        }
    }

    // ------------------------------------------------------------------------
    private void DrawChatOptions (Message message) {
        if(message == null) {
            Debug.LogError("Message null.");
            return;
        }
        if(!message.player) {
            Debug.LogError("Hecked up script config. NPC has chat options.");
            return;
        }
        if(!message.HasOptions()) {
            Debug.LogError("Attempting to draw options for message with no options.");
            return;
        }

        // if we've already chosen, draw a chat bubble
        if(message.optionChosen) {
            CreateChatBubbles(message, PlayerChatBubblePrefab);
        } else {
            // otherwise, draw the options
            for(int i = 0; i < message.options.Length; i++) {
                // draw bubble
                GameObject option = Instantiate(
                    MessageOptionPrefab,
                    ChatOptionsParent
                ) as GameObject;

                // setup bubble
                MessageButton messageButton = option.GetComponent<MessageButton>();
                
                // check if clue needs met
                if(message.OptionAvailable(i)) {
                    // set button text & hook up option function
                    messageButton.Text.text = message.options[i];
                    SetButtonListener(messageButton.Button, i);
                    //Debug.Log("created option [" + message.options[i] + "] with index " + i + " for message " + message.node);
                } else {
                    // mark it as unavilable
                    messageButton.Text.text = "[clue needed]";
                }
            }
        }
    }

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

        Debug.Log("selected option " + option + " for message " + message.node);

        // record in message that this option has been chosen
        message.optionChosen = true;
        message.optionSelection = option;
        message.messages = new string[1];
        message.messages[0] = message.options[option];

        // draw chosen message
        CreateChatBubbles(message, PlayerChatBubblePrefab);

        // destroy option bubbles
        foreach(Transform child in ChatOptionsParent.transform) {
            Destroy(child.gameObject);
        }

        // draw next chat
        MoveConversation();
    }

    // ------------------------------------------------------------------------
    private void CloseChat () {
        foreach(Transform child in ChatBubblesParent.transform) {
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
