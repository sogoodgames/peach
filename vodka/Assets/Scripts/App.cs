using UnityEngine;

public abstract class App : MonoBehaviour
{
    public bool IsOpen {
        get {return gameObject.activeInHierarchy;}
    }

    public virtual void Open() {
        gameObject.SetActive(true);
    }
    public virtual void Close() {
        gameObject.SetActive(false);
    }
}
