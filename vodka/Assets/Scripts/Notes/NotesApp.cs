
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesApp : App
{
    // managers
    public NotificationManager NotificationManager;

    // images
    public Sprite RudditSprite;
    public Sprite MessagesSprite;
    public Sprite NotesSprite;

    // cannot be bothered to put these in json, there's only 10
    private Dictionary<ClueID, string> clueNotes = new Dictionary<ClueID, string> () {
        {ClueID.Shoe, "That cute SGG cosplayer left one of his shoes at my place."},
        {ClueID.Pool, "Somebody pushed someone into the pool last night."},
        {ClueID.Band, "Somebody with the username ‘tegansara94’ posted an ad about that band that played last night. Who are they?"},
        {ClueID.Pizza, "Looks like Michael, the person who got pushed into the pool, was also eating pineapple pizza. I should ask him about it."},
        {ClueID.Poetry, "Courtney knows the creative writing majors who were at the party last night. Should I ask her about them?"},
        {ClueID.Cow, "Hey... this photo of the girl in a cow costume is from the party. I wonder who she is?"},
        {ClueID.Flirt, "Okay, apparently Kyle hit on the girl who did the booze run! Maybe he has her contact info?"},
        {ClueID.EmmaPhone, "Emma wants me to give her Taeyong’s contact info! I need to get in touch with him."},
        {ClueID.TaeyongPhone, "Taeyong wants to trade Jin’s number for the girl who did the beer run! I need to talk to her."}
    };

    public override void Open() {
        base.Open();
    }

    public override void Close() { 
        base.Close();
    }

    public void FoundClue (ClueID clue) {
        if(clueNotes.ContainsKey(clue)) {
            NotificationManager.Open(NotesSprite, clueNotes[clue]);
        }
    }
}
