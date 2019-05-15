using UnityEngine;
using UnityEngine.UI;

public class RudditPost : MonoBehaviour
{
    public Image ProfileImage;
    public Text TitleText;
    public Text UsernameText;
    public GameObject PostContent;

    public void ToggleOpenPost () {
        PostContent.SetActive(!PostContent.activeSelf);
    }
}
