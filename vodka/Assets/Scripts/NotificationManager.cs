using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    // UI references
    public GameObject NotificationUI;
    public Image Icon;
    public Text Text;

    // tuning
    public float LingerSeconds = 2.0f;

    // internal
    private float timeLeft = 0.0f;
    private bool linger = false;

    void Update () {
        if(linger && timeLeft >= 0) {
            timeLeft -= Time.deltaTime;
        } else {
            NotificationUI.SetActive(false);
            linger = false;
        }
    }

    public void Open (Sprite icon, string text) {
        Icon.sprite = icon;
        Text.text = text;
        timeLeft = LingerSeconds;
        linger = true;
        NotificationUI.SetActive(true);
    }
}