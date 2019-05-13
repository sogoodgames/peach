using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatApp : App
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public PhoneOS PhoneOS;
    public ChatSelectionScreen ChatSelectionScreen;
    public ChatScreen ChatScreen;

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public override void Open() {
        base.Open();
        // always reset to chat selection bc i'm lazy lol
        ChatSelectionScreen.Open(PhoneOS.ActiveChats);
        ChatScreen.Close();
    }

    // ------------------------------------------------------------------------
    public void OpenChatSelection () {
        ChatSelectionScreen.Open(PhoneOS.ActiveChats);
        ChatScreen.Close();
    }

    // ------------------------------------------------------------------------
    public void OpenChat (Chat c) {
        ChatSelectionScreen.Close();
        ChatScreen.Open(c);
    }
}
