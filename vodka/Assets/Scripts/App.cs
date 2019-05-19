using UnityEngine;

public abstract class App : MonoBehaviour
{
    public PhoneOS PhoneOS;
    public Sprite Icon;

    public bool IsOpen {
        get {return gameObject.activeInHierarchy;}
    }

    public virtual void Open() {
        gameObject.SetActive(true);
    }

    public virtual void Close() {
        gameObject.SetActive(false);
    }

    public virtual void Return() {
        PhoneOS.GoHome();
    }
}
