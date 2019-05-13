using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PhoneOS : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public List<App> Apps;
    public App HomeApp;
    public List<string> ChatFileNames;

    private List<Chat> m_allChats;
    // increases every time we unlock a new chat
    private int m_chatCounter = 0;

    // ------------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------------
    public List<Chat> ActiveChats {
        get {
            // only add chats that are available
            List<Chat> activeChats = new List<Chat>();
            foreach(Chat c in m_allChats) {
                if(c.unlocked) {
                    activeChats.Add(c);
                }
            }
            // sort by when they were acquired, oldest first
            activeChats = activeChats.OrderBy(c => c.order).ToList();
            return activeChats;
        }
    }

    // ------------------------------------------------------------------------
    // Methods: Monobehaviour
    // ------------------------------------------------------------------------
    void Awake () {
        LoadChats();
    }
    
    // ------------------------------------------------------------------------
    // Methods: Phone navigation
    // ------------------------------------------------------------------------
    public void OpenApp(App app) {
        CloseAllApps();
        app.Open();
    }

    // ------------------------------------------------------------------------
    public void GoHome() {
        CloseAllApps();
        HomeApp.Open();
    }

    // ------------------------------------------------------------------------
    // Methods: Clues
    // ------------------------------------------------------------------------
    public bool ClueRequirementMet (ClueID id) {
        if(id == ClueID.NoClue) return true;
        // TODO: map of clues to fulfillment 
        else return false;
    }

    // ------------------------------------------------------------------------
    // Methods: Private
    // ------------------------------------------------------------------------
    private void CloseAllApps() {
        foreach(App app in Apps) {
            if(app.IsOpen) {
                app.Close();
            }
        }
    }

    // ------------------------------------------------------------------------
    private void LoadChats() {
        m_allChats = new List<Chat>();

        foreach(string fileName in ChatFileNames) {
            var textFile = Resources.Load<TextAsset>(fileName);
            if(textFile == null) {
                Debug.LogError("unable to find file: " + fileName);
                break;
            }

            string text = textFile.text;
            if(!string.IsNullOrEmpty(text)) {
                ChatSerializable chatSer = JsonUtility.FromJson<ChatSerializable>(text);
                Chat chat = new Chat(chatSer);

                if(!chat.HasMessages) {
                    Debug.LogWarning("Chat empty: " + fileName);
                } else {
                    // if it's unlocked from the start, increase our chat counter
                    if(chat.unlocked) {
                        chat.order = m_chatCounter;
                        m_chatCounter++;
                    }

                    m_allChats.Add(chat);
                    //Debug.Log("added chat: " + chat.friend.ToString() + "; order: " + chat.order);
                }
            } else {
                Debug.LogError("file empty: " + fileName);
                break;
            }
        }
    }
}
