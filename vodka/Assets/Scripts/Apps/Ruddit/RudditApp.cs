using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RudditApp : App
{
    public PhoneOS PhoneOS;

    public Transform ForumPostParent;
    public GameObject ForumPostPrefab;

    public override void Open() {
        base.Open();

        foreach(ForumPost post in PhoneOS.ActiveForumPosts) {
            GameObject postObj = Instantiate(
                ForumPostPrefab,
                ForumPostParent)
            as GameObject;

            RudditPost ruddit = postObj.GetComponent<RudditPost>();
            if(ruddit) {
                ruddit.TitleText.text = post.Title;
                ruddit.UsernameText.text = "u/" + post.Username;
                ruddit.MetaInfoText.text = post.NumComments
                                        + " comments / posted "
                                        + post.Time
                                        + " hours ago";
                ruddit.BodyText.text = post.Body;
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
