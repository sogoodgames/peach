using UnityEngine;
using UnityEngine.UI;

public class ChatBubbleUI : MonoBehaviour {
    public Text Text;
    public Image ContentImage;

    public void SetImageContent(Message message, PhoneOS os) {
        if(message.Image < 0) {
            return;
        }

        Sprite img = os.PhotoAssets[message.Image];
        if(img) {
            ContentImage.sprite = img;
            ContentImage.gameObject.SetActive(true);
            ContentImage.SetNativeSize();
            ContentImage.rectTransform.sizeDelta *= message.ImageHeight;
        }
    }
}