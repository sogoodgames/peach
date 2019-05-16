using System;   // serializable

public class ForumPostData
{
    // ------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------
    // the order they were unlocked in, so that they appear in your
    // list of messages in a way that makes sense
    public int Order = 0;

    // ------------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------------
    private string m_username; // poster username
    public string Username {get{return m_username;}}

    private string m_title; // post title
    public string Title {get{return m_title;}}

    private ClueID m_clueGiven; // the clue given (if any)
    public ClueID ClueGiven {get{return m_clueGiven;}}

    private string m_body; // text in the post
    public string Body {get{return m_body;}}

    private int m_numComments; // number of comments
    public int NumComments {get{return m_numComments;}}

    private int m_time; // minutes ago it was posted
    public int Time {get{return m_time;}}

    private bool m_unlocked; // unlocked from beginning
    public bool Unlocked {get{return m_unlocked;}}

    // ------------------------------------------------------------------------
    // Methods
    // ------------------------------------------------------------------------
    public ForumPostData (ForumPostSerializable post) {
        m_username = post.username;
        m_title = post.title;
        m_clueGiven = post.clueGiven;
        m_body = post.body;
        m_numComments = post.numComments;
        m_time = post.time;
        m_unlocked = post.unlocked;
    }

    // ------------------------------------------------------------------------
    public bool Empty () {
        return string.IsNullOrEmpty(m_body);
    }
}
