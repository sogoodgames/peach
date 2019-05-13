using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatApp : App
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public PhoneOS PhoneOS;
    public GameObject ChatSelectionScreen;
    public GameObject ChatScreen;

    public Transform ChatButtonsParent;
    public GameObject ChatButtonPrefab;

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public override void Open() {
        base.Open();
        // always reset to chat selection bc i'm lazy lol
        OpenChatSelection();
    }

    // ------------------------------------------------------------------------
    public void OpenChatSelection () {
        ChatScreen.SetActive(false);

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
        //ChatSelectionScreen.SetActive(false);

        Debug.Log("want to open chat: " + c.friend.ToString());

        //ChatScreen.SetActive(true);
    }
}
