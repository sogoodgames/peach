using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSelectionScreen : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public Transform ChatButtonsParent;
    public GameObject ChatButtonPrefab;

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public void Open (List<Chat> chats) {
        gameObject.SetActive(true);

        // populate list of chat buttons
        // we do this every time we open the app in case it's changed
        foreach(Chat chat in chats) {
            GameObject chatButtonObj = Instantiate(
                ChatButtonPrefab,
                ChatButtonsParent
            ) as GameObject;

            OpenChatButton chatButton = chatButtonObj.GetComponent<OpenChatButton>();
            if(chatButton) {
                chatButton.NameText.text = chat.friend.ToString();
            } else {
                Debug.LogError("Chat Button Prefab does not contain an OpenChatButton.");
            }
        }
    }

    // ------------------------------------------------------------------------
    public void Close () {
        gameObject.SetActive(false);

        // clear open chat buttons
    }
}
