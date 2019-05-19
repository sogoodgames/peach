using UnityEngine;
using UnityEngine.UI;

public class ChatAttachment : MonoBehaviour {
    public Image Image;

    public void Open (Sprite img) {
        Image.sprite = img;
        Image.SetNativeSize();
        Image.rectTransform.sizeDelta *= 5.0f;
        gameObject.SetActive(true);
    }
    
    public void Close () {
        gameObject.SetActive(false);
    }
}