using UnityEngine;
using UnityEngine.UI;

public class ChatAttachment : MonoBehaviour {
    public Image Image;

    public void Open (Sprite img/*, float width, float height*/) {
        Image.sprite = img;
        Image.preserveAspect = true;
        //Image.SetNativeSize();
        //Image.rectTransform.sizeDelta = new Vector2(width, height);
        gameObject.SetActive(true);
    }
    
    public void Close () {
        gameObject.SetActive(false);
    }
}