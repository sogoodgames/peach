using System;   // serializable
using System.Collections.Generic;

public class Message {
    // ------------------------------------------------------------------------
    // Public Variables
    // ------------------------------------------------------------------------
    // the selected option (if any)
    public int OptionSelection;

    // all of the messages to play
    public string[] Messages;

    // ------------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------------
    // identifier for this message
    private int node;
    public int Node {get{return node;}}

    // is this the player speaking or not
    private bool player;
    public bool Player {get{return player;}}

    // the clue this message gives (if any)
    private ClueID clueGiven;
    public ClueID ClueGiven {get{return clueGiven;}}

    // converstaion options (leave empty if it's a non-player node or this node has messages)
    private string[] options;
    public string[] Options {get{return options;}}

    // clue required to selection the option (by clue ID)
    private ClueID[] clueNeeded;
    public ClueID[] ClueNeeded {get{return clueNeeded;}}

    // option destinations (by message node)
    private int[] branch;
    public int[] Branch {get{return branch;}}

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public Message (MessageSerializable serializedMessage) {
        node = serializedMessage.node;
        player = serializedMessage.player;
        clueGiven = serializedMessage.clueGiven;
        Messages = serializedMessage.messages;
        options = serializedMessage.options;
        clueNeeded = serializedMessage.clueNeeded;
        branch = serializedMessage.branch;

        OptionSelection = -1;
    }

    // ------------------------------------------------------------------------
    public bool MadeSelection () {
        if(!HasOptions()) return true;
        return OptionSelection >= 0;
    }

    // ------------------------------------------------------------------------
    public bool HasOptions () {
        return options != null && options.Length > 0;
    }
}

public class Chat {
    // ------------------------------------------------------------------------
    // Public Variables
    // ------------------------------------------------------------------------
    // the order they were unlocked in, so that they appear in your
    // list of messages in a way that makes sense
    public int order = 0;

    // only characters that are available right when the game starts
    // will have this as 'true' in the json
    // otherwise, the flag is set by gameplay
    public bool unlocked;

    // only the messages you've visited so far
    public List<Message> visitedMessages;

    // the index (in 'messages') of the last node read
    public int lastNode = 0;

    // whether or not the convo is finished
    public bool finished;

    // ------------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------------
    // the conversation partner
    private Friend friend;
    public Friend Friend {get{return friend;}}

    // all of the messages you trade with them
    private Message[] messages;
    public bool HasMessages {get{return messages != null && messages.Length > 0;}}

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public Chat (ChatSerializable serializedChat) {
        friend = serializedChat.friend;
        unlocked = serializedChat.unlocked;

        order = 0;
        lastNode = 0;

        if(serializedChat.messages == null) {
            return;
        }

        messages = new Message[serializedChat.messages.Length];
        for (int i = 0; i < messages.Length; i++) {
            messages[i] = new Message(serializedChat.messages[i]);
        }

        visitedMessages = new List<Message>();
        visitedMessages.Add(messages[0]);
    }

    // ------------------------------------------------------------------------
    public Message GetMessage(int n) {
        foreach(Message m in messages) {
            if(m.Node == n) {
                return m;
            }
        }
        return null;
    }
}
