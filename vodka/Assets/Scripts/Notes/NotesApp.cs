using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesApp : App
{
    // UI refs
    public Transform NotesParent;
    public GameObject NotePrefab;

    // cannot be bothered to put these in json, there's only 10
    public Dictionary<ClueID, string> clueNotes = new Dictionary<ClueID, string> () {
        {ClueID.Shoe, "That cute SGG cosplayer left one of his shoes at my place."},
        {ClueID.Pool, "Somebody pushed someone into the pool last night."},
        {ClueID.Band, "Somebody with the username ‘tegansara94’ posted an ad about that band that played last night. Who are they?"},
        {ClueID.Pizza, "Looks like Michael, the person who got pushed into the pool, was also eating pineapple pizza. I should ask him about it."},
        {ClueID.Poetry, "tegansara94 knows the graphic design majors who were at the party last night. Should I ask them about them?"},
        {ClueID.Cow, "Hey... this photo of the girl in a cow costume is from the party. I wonder who she is?"},
        {ClueID.Flirt, "Okay, apparently Kyle hit on the girl who did the booze run! Maybe he has her contact info?"},
        {ClueID.EmmaPhone, "Emma wants me to give her Taeyong’s contact info! I need to get in touch with him."},
        {ClueID.TaeyongPhone, "Taeyong wants to trade Jin’s number for the girl who did the beer run! I need to talk to her."}
    };

    public override void Open() {
        base.Open();
        PopulateNotes(); 
    }

    public override void OnCloseAnimationFinished () {
        base.OnCloseAnimationFinished();
        foreach(Transform child in NotesParent.transform) {
            Destroy(child.gameObject);
        }
    }

    private void PopulateNotes () {
        foreach(ClueID clue in clueNotes.Keys) {
            if(PhoneOS.ClueRequirementMet(clue)) {
                GameObject noteObj = Instantiate(NotePrefab, NotesParent);
                NoteUI noteUI = noteObj.GetComponent<NoteUI>();
                if(noteUI) {
                    noteUI.Text.text = clueNotes[clue];
                }
            }
        }
    }
}
