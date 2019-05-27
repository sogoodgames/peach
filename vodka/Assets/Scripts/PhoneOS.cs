using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public enum Friend {
    Courtney = 0,
    Emma = 1,
    Jin = 2,
    Kyle = 3,
    Lucien = 4,
    Matt = 5,
    Melody = 6,
    Michael = 7,
    Riley = 8,
    Taeyong = 9
}

public enum ClueID {
    NoClue = 0,
    Pool = 1, // note 2
    Band = 2, // note 3
    Pizza = 3, // note 4
    Poetry = 4, // note 5
    Cow = 5, // note 6
    Flirt = 6, // note 7
    EmmaPhone = 7, // note 8
    TaeyongPhone = 8, // note 9
    CourtneyPhone = 9,
    JinPhone = 10,
    MichaelPhone = 11,
    MelodyPhone = 12,
    Shoe = 13,
    KylePhone = 14,
    MattPhone = 15,
    LucienPhone = 16
}

public class PhoneOS : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    public NotificationManager NotificationManager;
    public NotesApp NotesApp;
    public ChatApp ChatApp;
    public GameObject ReturnButton;
    public List<App> Apps;
    public App HomeApp;
    public List<TextAsset> ChatTextAssets;
    public List<TextAsset> ForumPostTextAssets;
    public List<Sprite> UserIconAssets;
    public List<Sprite> PhotoAssets;
    public Sprite EmmaEndingPhoto;
    public Sprite JinEndingPhoto;

    public AnimationCurve AppAnimationCurve;
    public float AppAnimationSpeed = 2000;
    public float AppAnimationTime = 0.5f;
    public float AppStartX = 1080.0f;

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
                if(ClueRequirementMet(p.ClueNeeded)) {
                    activePosts.Add(p);
                }
            }
            // sort by when they were acquired, oldest first
            activePosts = activePosts.OrderBy(p => p.Order).ToList();
            return activePosts;
        }
    }

    // ------------------------------------------------------------------------
    // Private Variables
    // ------------------------------------------------------------------------
    // clues really should have been a class
    // with corresponding json
    // but it's too late
    private Dictionary<ClueID, string> PhoneNumberClueNames = new Dictionary<ClueID, string>() {
        {ClueID.EmmaPhone, "Emma"},
        {ClueID.TaeyongPhone, "Taeyong"},
        {ClueID.CourtneyPhone, "Courtney"},
        {ClueID.JinPhone, "Jin"},
        {ClueID.MichaelPhone, "Michael"},
        {ClueID.MelodyPhone, "Melody"},
        {ClueID.KylePhone, "Kyle"},
        {ClueID.LucienPhone, "Lucien"},
        {ClueID.MattPhone, "Matt"}
    };

    // ------------------------------------------------------------------------
    // Methods: Monobehaviour
    // ------------------------------------------------------------------------
    void Awake () {
        Screen.SetResolution(480, 848, false);

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
            {ClueID.Shoe, false},
            {ClueID.KylePhone, false},
            {ClueID.MattPhone, false},
            {ClueID.LucienPhone, false}
        };

        LoadChats();
        LoadForumPosts();
    }

    void OnEnable () {
        Chat rileyChat = m_allChats[0];
        foreach(Chat c in m_allChats) {
            if(c.Friend == Friend.Riley) {
                rileyChat = c;
                break;
            }
        }
        ChatApp.OpenChat(rileyChat);
        m_activeApp = ChatApp;
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
        // don't do anything if we already knew
        if(m_clueLockStates[id] == true) {
            return;
        }

        // if this is a phone number, send a new contact notif
        if(PhoneNumberClueNames.ContainsKey(id)) {
            NotificationManager.NewContactNotif(PhoneNumberClueNames[id]);
        } else {
            // otherwise, send generic clue notif
            NotificationManager.FoundClueNotif(id); // really should send an event but meh
        }

        // if it unlocks a ruddit post, send a ruddit notif
        foreach(ForumPostData post in m_allForumPosts) {
            if(post.ClueNeeded == id) {
                NotificationManager.ForumPostNotif(post);
            }
        }

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
                    if(ClueRequirementMet(post.ClueNeeded)) {
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
