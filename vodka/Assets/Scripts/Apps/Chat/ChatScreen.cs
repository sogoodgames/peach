using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public Text NameText;
    public Transform ChatBubblesParent;

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public void Open(Chat chat) {
        gameObject.SetActive(true);

        // set name 

        // populate chat bubbles
    }

    // ------------------------------------------------------------------------
    public void Close () {
        gameObject.SetActive(false);

        // clear chat bubbles
    }
}
