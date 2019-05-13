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

[Serializable]
public class Chat
{
    public Friend friend;
    // the order they were unlocked in, so that they appear in your
    // list of messages in a way that makes sense
    public int order;
    // only characters that are available right when the game starts
    // will have this as 'true' in the json
    // otherwise, the flag is set by gameplay
    public bool unlocked;
    public Chat (Friend frnd, bool u) {
        friend = frnd;
        order = 0;
        unlocked = u;
    }
}
