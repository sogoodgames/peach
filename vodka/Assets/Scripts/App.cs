using UnityEngine;

public abstract class App : MonoBehaviour
{
    public PhoneOS PhoneOS;
    public Sprite Icon;
    public bool DoSlideAnimation = true;

    protected bool animate;
    private float animTime;
    private int xDir;
    private bool waitForClose;

    public bool IsOpen {
        get {return gameObject.activeInHierarchy;}
    }

    protected virtual void Update () {
        if(animate) {
            float animTimeLinear = animTime / PhoneOS.AppAnimationTime;
            float animCurve = PhoneOS.AppAnimationCurve.Evaluate(animTimeLinear);
            transform.Translate(
                xDir * animCurve * PhoneOS.AppAnimationSpeed * Time.deltaTime,
                0, 0
            );
            animTime += Time.deltaTime;

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
        animTime = 0;
        transform.localPosition = new Vector3(PhoneOS.AppStartX, transform.localPosition.y, transform.localPosition.z);
        xDir = -1;
    }

    public void PlaySlideOutAnimation () {
        animate = true;
        animTime = 0;
        waitForClose = true;
        xDir = 1;
    }
}
