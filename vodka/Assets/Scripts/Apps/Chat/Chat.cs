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
public class Message {
    // identifier for this message
    int node;
    // is this the player speaking or not
    bool player;
    // messages to play consecutively (leave empty if there's options)
    string[] messages;
    // converstaion options (leave empty if it's a non-player node or this node has messages)
    string[] options;
    // option destinations (by message node)
    int[] branch;

    public Message (int n, bool p, string[] m, string[] op, int[] b) {
        node = n;
        player = p;
        messages = m;
        options = op;
        branch = b;
    }
}

[Serializable]
public class Chat
{
    // the conversation partner
    public Friend friend;

    // the order they were unlocked in, so that they appear in your
    // list of messages in a way that makes sense
    public int order;

    // only characters that are available right when the game starts
    // will have this as 'true' in the json
    // otherwise, the flag is set by gameplay
    public bool unlocked;

    // all of the messages you trade with them
    public Message[] messages;

    public Chat (Friend frnd, bool u, Message[] msg) {
        friend = frnd;
        order = 0;
        unlocked = u;
        messages = msg;
    }
}
