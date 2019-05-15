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
    public List<TextAsset> ChatTextAssets;
    public List<TextAsset> ForumPostTextAssets;

    private List<Chat> m_allChats;
    private int m_chatCounter = 0; // increases every time we unlock a new chat
    private App m_activeApp;

    private List<ForumPost> m_allForumPosts;
    private int m_postCounter = 0; // increases every time we unlock a new forum post
    private ForumPost m_activeForumPost;

    private Dictionary<ClueID, bool> m_clueLockStates;

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

    public List<ForumPost> ActiveForumPosts {
        get {
            // only add posts that are available
            List<ForumPost> activePosts = new List<ForumPost>();
            foreach(ForumPost p in m_allForumPosts) {
                if(p.Unlocked) {
                    activePosts.Add(p);
                }
            }
            // sort by when they were acquired, oldest first
            activePosts = activePosts.OrderBy(p => p.Order).ToList();
            return activePosts;
        }
    }

    // ------------------------------------------------------------------------
    // Methods: Monobehaviour
    // ------------------------------------------------------------------------
    void Awake () {
        m_clueLockStates = new Dictionary<ClueID, bool> {
            {ClueID.NoClue, true},
            {ClueID.Pool, false},
            {ClueID.Band, false},
            {ClueID.Pizza, false},
            {ClueID.Poetry, false},
            {ClueID.Cow, false},
            {ClueID.Flirt, false},
            {ClueID.EmmaPhone, false},
            {ClueID.TaeyongPhone, false},
            {ClueID.CourtneyPhone, false},
            {ClueID.JinPhone, false},
            {ClueID.MichaelPhone, false},
            {ClueID.MelodyPhone, false},
        };

        LoadChats();
        LoadForumPosts();
    }
    
    // ------------------------------------------------------------------------
    // Methods: Phone navigation
    // ------------------------------------------------------------------------
    public void OpenApp (App app) {
        CloseAllApps();
        app.Open();
        m_activeApp = app;
    }

    // ------------------------------------------------------------------------
    public void GoHome () {
        CloseAllApps();
        HomeApp.Open();
        m_activeApp = HomeApp;
    }

    // ------------------------------------------------------------------------
    public void Return () {
        m_activeApp.Return();
    }

    // ------------------------------------------------------------------------
    // Methods: Clues
    // ------------------------------------------------------------------------
    public bool ClueRequirementMet (ClueID id) {
        return m_clueLockStates[id];
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

        foreach(TextAsset textAsset in ChatTextAssets) {
            string text = textAsset.text;
            if(!string.IsNullOrEmpty(text)) {
                ChatSerializable chatSer = JsonUtility.FromJson<ChatSerializable>(text);
                Chat chat = new Chat(chatSer);

                if(!chat.HasMessages) {
                    Debug.LogWarning("Chat empty: " + textAsset.name);
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
                Debug.LogError("file empty: " + textAsset.name);
                break;
            }
        }
    }

    // ------------------------------------------------------------------------
    private void LoadForumPosts() {
        m_allForumPosts = new List<ForumPost>();

        foreach(TextAsset textAsset in ForumPostTextAssets) {
            string text = textAsset.text;
            if(!string.IsNullOrEmpty(text)) {
                ForumPostSerializable postSer = JsonUtility.FromJson<ForumPostSerializable>(text);
                ForumPost post = new ForumPost(postSer);

                if(post.Empty()) {
                    Debug.LogWarning("Forum Post empty: " + textAsset.name);
                } else {
                    // if it's unlocked from the start, increase our post counter
                    if(post.Unlocked) {
                        post.Order = m_postCounter;
                        m_postCounter++;
                    }

                    m_allForumPosts.Add(post);
                    Debug.Log("added post: " + post.Username + "; order: " + post.Order);
                }
            } else {
                Debug.LogError("file empty: " + textAsset.name);
                break;
            }
        }
    }
}
