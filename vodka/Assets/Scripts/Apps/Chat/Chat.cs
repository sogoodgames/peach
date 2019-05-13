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
public class Message {
    // identifier for this message
    public int node;

    // is this the player speaking or not
    public bool player;

    // the clue this message gives (if any)
    public ClueID clueGiven;

    // messages to play consecutively (leave empty if there's options)
    public string[] messages;

    // !!! all of these map to each other by index !!!
    // converstaion options (leave empty if it's a non-player node or this node has messages)
    public string[] options;
    // clue required to selection the option (by clue ID)
    public ClueID[] clueNeeded;
    // clue requirement fulfilled
    public bool[] clueFulfilled;
    // option destinations (by message node)
    public int[] branch;
    // wheter or not an option was chosen
    public bool optionChosen = false;
    // the option picked
    public int optionSelection = -1;

    public Message (int n, bool p, string[] m, string[] op, int[] b) {
        node = n;
        player = p;
        messages = m;
        options = op;
        branch = b;
    }

    public bool HasOptions () {
        return options != null && options.Length > 0;
    }

    public bool OptionAvailable(int i) {
        if(clueNeeded == null
           || i >= clueNeeded.Length
           || clueFulfilled == null 
           || i >= clueFulfilled.Length
        ) {
            return true;
        }
        return clueNeeded[i] != ClueID.NoClue || clueFulfilled[i];
    }
}

[Serializable]
public class Chat
{
    // the conversation partner
    public Friend friend;

    // the order they were unlocked in, so that they appear in your
    // list of messages in a way that makes sense
    public int order = 0;

    // only characters that are available right when the game starts
    // will have this as 'true' in the json
    // otherwise, the flag is set by gameplay
    public bool unlocked;

    // all of the messages you trade with them
    public Message[] messages;
    public bool HasMessages {get{return messages != null && messages.Length > 0;}}

    // only the messages you've visited so far
    public List<Message> visitedMessages;

    // the index (in 'messages') of the last node read
    public int lastNode = 0;

    public Chat (Friend frnd, bool u, Message[] msg) {
        friend = frnd;
        unlocked = u;
        messages = msg;
    }

    public void Init () {
        visitedMessages = new List<Message>();
        visitedMessages.Add(messages[0]);
        order = 0;
        lastNode = 0;
    }

    public Message GetMessage(int n) {
        foreach(Message m in messages) {
            if(m.node == n) {
                return m;
            }
        }
        return null;
    }
}
