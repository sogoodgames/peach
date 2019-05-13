using System;   // serializable
using System.Collections.Generic;

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
    Pool = 1,
    Band = 2,
    Pizza = 3,
    Poetry = 4,
    Cow = 5,
    Flirt = 6,
    EmmaPhone = 7,
    TaeyongPhone = 8
}

[Serializable]
public class MessageSerializable {
    public int node; // the ID for this message
    public bool player; // whether or not it's the player talking
    public ClueID clueGiven; // the clue given (if any)
    public string[] messages; // the text for the messages sent

    // all of the following map by index
    public string[] options; // the text options
    public ClueID[] clueNeeded; // the clues needed for each option
    public int[] branch; // the destination for each option (or just the node)
}

[Serializable]
public class ChatSerializable {
    public Friend friend; // the person you're talking to
    public bool unlocked; // whether or not it's unlocked from the start
    public MessageSerializable[] messages; // all of the messages
}