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

    public void SetPhotoContent (int photoIndex, PhoneOS os) {
        if(photoIndex > -1) {
            Sprite img = os.PhotoAssets[photoIndex];
            if(img) {
                ContentImage.sprite = img;
                ContentImage.gameObject.SetActive(true);
                ContentImage.SetNativeSize();
                ContentImage.rectTransform.sizeDelta *= 0.5f;
            }
        }
    }
}
