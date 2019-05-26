using UnityEngine;

public abstract class App : MonoBehaviour
{
    public PhoneOS PhoneOS;
    public Sprite Icon;
    public bool DoSlideAnimation = true;

    protected bool animate;
    private float animSecLeft;
    private int xDir;
    private bool waitForClose;

    public bool IsOpen {
        get {return gameObject.activeInHierarchy;}
    }

    protected virtual void Update () {
        if(animate) {
            transform.Translate(xDir * PhoneOS.AppAnimationSpeed * Time.deltaTime, 0, 0);

            bool reachedDestination = false;
            if(waitForClose) {
                reachedDestination = transform.localPosition.x >= PhoneOS.AppStartX;
            } else {
                reachedDestination = transform.localPosition.x <= 0.0f;
            }

            if(reachedDestination) {
                transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z); 
                animate = false;

                if(waitForClose) {
                    OnCloseAnimationFinished();
                }
            }
        }
    }

    public virtual void Open() {
        gameObject.SetActive(true);
        if(DoSlideAnimation) {
            PlaySlideInAnimation();
        }
    }

    public virtual void Close() {
        if(DoSlideAnimation) {
            PlaySlideOutAnimation();
        } else {
            OnCloseAnimationFinished();
        }
    }

    public virtual void OnCloseAnimationFinished() {
        gameObject.SetActive(false);
        waitForClose = false;
    }

    public virtual void Return() {
        PhoneOS.GoHome();
    }

    public void PlaySlideInAnimation () {
        animate = true;
        transform.localPosition = new Vector3(PhoneOS.AppStartX, transform.localPosition.y, transform.localPosition.z);
        xDir = -1;
    }

    public void PlaySlideOutAnimation () {
        animate = true;
        waitForClose = true;
        xDir = 1;
    }
}
