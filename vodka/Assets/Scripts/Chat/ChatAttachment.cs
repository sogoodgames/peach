using UnityEngine;
using UnityEngine.UI;

public class ChatAttachment : MonoBehaviour {
    public Image Image;

    public void Open (Sprite img, float height) {
        Image.sprite = img;
        Image.SetNativeSize();
        Image.rectTransform.sizeDelta *= height;
        gameObject.SetActive(true);
    }
    
    public void Close () {
        gameObject.SetActive(false);
    }
}