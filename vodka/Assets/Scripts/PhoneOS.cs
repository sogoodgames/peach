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
    public List<Sprite> UserIconAssets;
    public List<Sprite> PhotoAssets;

    private List<Chat> m_allChats;
    private int m_chatCounter = 0; // increases every time we unlock a new chat
    private App m_activeApp;

    private List<ForumPostData> m_allForumPosts;
    private int m_postCounter = 0; // increases every time we unlock a new forum post
    private ForumPostData m_activeForumPost;

    private Dictionary<ClueID, bool> m_clueLockStates;

    // ------------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------------
    public List<Chat> ActiveChats {
        get {
            // only add chats that are available
            List<Chat> activeChats = new List<Chat>();
            foreach(Chat c in m_allChats) {
                if(ClueRequirementMet(c.ClueNeeded)) {
                    activeChats.Add(c);
                }
            }
            // sort by when they were acquired, oldest first
            activeChats = activeChats.OrderBy(c => c.order).ToList();
            return activeChats;
        }
    }

    public List<ForumPostData> ActiveForumPosts {
        get {
            // only add posts that are available
            List<ForumPostData> activePosts = new List<ForumPostData>();
            foreach(ForumPostData p in m_allForumPosts) {
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
    public void FoundClue (ClueID id) {
        m_clueLockStates[id] = true;
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
                    if(chat.ClueNeeded == ClueID.NoClue) {
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
        m_allForumPosts = new List<ForumPostData>();

        foreach(TextAsset textAsset in ForumPostTextAssets) {
            string text = textAsset.text;
            if(!string.IsNullOrEmpty(text)) {
                ForumPostSerializable postSer = JsonUtility.FromJson<ForumPostSerializable>(text);
                ForumPostData post = new ForumPostData(postSer);

                if(post.Empty()) {
                    Debug.LogWarning("Forum Post empty: " + textAsset.name);
                } else {
                    // if it's unlocked from the start, increase our post counter
                    if(post.Unlocked) {
                        post.Order = m_postCounter;
                        m_postCounter++;
                    }

                    m_allForumPosts.Add(post);
                    //Debug.Log("added post: " + post.Username + "; order: " + post.Order);
                }
            } else {
                Debug.LogError("file empty: " + textAsset.name);
                break;
            }
        }
    }
}
