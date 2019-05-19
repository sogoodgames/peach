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

    // cannot be bothered to put these in json, there's only 10
    private Dictionary<ClueID, string> clueNotes = new Dictionary<ClueID, string> () {
        {ClueID.Shoe, "That cute SGG cosplayer left one of his shoes at my place."},
        {ClueID.Pool, "Somebody pushed someone into the pool last night."},
        {ClueID.Band, "Somebody with the username ‘tegansara94’ posted an ad about that band that played last night. Who are they?"},
        {ClueID.Pizza, "Looks like Michael, the person who got pushed into the pool, was also eating pineapple pizza. I should ask him about it."},
        {ClueID.Poetry, "Courtney knows the graphic design majors who were at the party last night. Should I ask her about them?"},
        {ClueID.Cow, "Hey... this photo of the girl in a cow costume is from the party. I wonder who she is?"},
        {ClueID.Flirt, "Okay, apparently Kyle hit on the girl who did the booze run! Maybe he has her contact info?"},
        {ClueID.EmmaPhone, "Emma wants me to give her Taeyong’s contact info! I need to get in touch with him."},
        {ClueID.TaeyongPhone, "Taeyong wants to trade Jin’s number for the girl who did the beer run! I need to talk to her."}
    };

    void Update () {
        if(linger && timeLeft >= 0) {
            timeLeft -= Time.deltaTime;
        } else {
            Close();
        }
    }

    public void FoundClueNotif (ClueID id) {
        if(!clueNotes.ContainsKey(id)) {
            return;
        }
        Icon.sprite = NoteApp.Icon;
        Text.text = clueNotes[id];

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