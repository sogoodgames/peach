using UnityEngine;
using UnityEngine.UI;

public class ForumPostUI : MonoBehaviour
{
    public Button OpenPostButton;
    public Image ProfileImage;
    public Image ContentImage;
    public Text TitleText;
    public Text UsernameText;
    public Text MetaInfoText;
    public Text BodyText;

    public GameObject PostContent;

    public void ToggleOpenPost () {
        PostContent.SetActive(!PostContent.activeSelf);
    }

    public void SetPhotoContent (ForumPostData post, PhoneOS os) {
        if(post.Photo > -1) {
            Sprite img = os.PhotoAssets[post.Photo];
            if(img) {
                ContentImage.sprite = img;
                ContentImage.gameObject.SetActive(true);
                ContentImage.SetNativeSize();
                ContentImage.rectTransform.sizeDelta *= post.ImageHeight;
            }
        }
    }
}
