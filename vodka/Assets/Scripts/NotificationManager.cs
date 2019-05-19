using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    // UI references
    public GameObject NotificationUI;
    public Image Icon;
    public Text Text;
    public Button Button;
    public NotesApp NotesApp;

    // game references
    public ChatApp ChatApp;
    public NotesApp NoteApp;
    public ForumApp ForumApp;

    // tuning
    public float LingerSeconds = 2.0f;
    public string NewContactNotifText = "New contact: ";

    // internal
    private float timeLeft = 0.0f;
    private bool linger = false;

    void Update () {
        if(linger && timeLeft >= 0) {
            timeLeft -= Time.deltaTime;
        } else {
            Close();
        }
    }

    public void FoundClueNotif (ClueID id) {
        if(!NotesApp.clueNotes.ContainsKey(id)) {
            return;
        }
        Icon.sprite = NoteApp.Icon;
        Text.text = NotesApp.clueNotes[id];

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(NoteApp.Open);

        Open();
    }

    public void NewContactNotif (string friendName) {
        Icon.sprite = ChatApp.Icon;
        Text.text = NewContactNotifText + friendName;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(ChatApp.Open);

        Open();
    }

    private void Open () {
        timeLeft = LingerSeconds;
        linger = true;
        NotificationUI.SetActive(true);
    }

    private void Close () {
        linger = false;
        NotificationUI.SetActive(false);
        Button.onClick.RemoveAllListeners();
    }
}