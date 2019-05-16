using System;   // serializable

[Serializable]
public class ForumPostSerializable {
    public string username; // poster username
    public string title; // post title
    public ClueID clueGiven; // the clue given (if any)
    public string body; // text in the post
    public int numComments; // number of comments
    public int time; // minutes ago it was posted
    public bool unlocked; // unlocked from beginning
    public int icon; // icon file
    public int photo; // post image file (optional)
}