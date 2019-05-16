using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForumApp : App
{
    public Transform ForumPostParent;
    public GameObject ForumPostPrefab;

    public override void Open() {
        base.Open();

        foreach(ForumPostData post in PhoneOS.ActiveForumPosts) {
            GameObject postObj = Instantiate(
                ForumPostPrefab,
                ForumPostParent)
            as GameObject;

            ForumPostUI Forum = postObj.GetComponent<ForumPostUI>();
            if(Forum) {
                Forum.TitleText.text = post.Title;
                Forum.UsernameText.text = "u/" + post.Username;
                Forum.MetaInfoText.text = post.NumComments
                                        + " comments / posted "
                                        + post.Time
                                        + " hours ago";
                Forum.BodyText.text = post.Body;
            }
        }
    }

    public override void Close() { 
        base.Close();

        foreach(Transform t in ForumPostParent.transform) {
            Destroy(t.gameObject);
        }
    }
}
