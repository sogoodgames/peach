using System.Collections.Generic;
using UnityEngine;

public class PhoneOS : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // Apps
    // ------------------------------------------------------------------------
    public List<App> Apps;
    public App HomeApp;
    
    // ------------------------------------------------------------------------
    // Methods: Phone navigation
    // ------------------------------------------------------------------------
    public void OpenApp(App app) {
        CloseAllApps();
        app.Open();
    }

    public void GoHome() {
        CloseAllApps();
        HomeApp.Open();
    }

    // ------------------------------------------------------------------------
    // Methods: Private
    // ------------------------------------------------------------------------
    private void CloseAllApps() {
        foreach(App app in Apps) {
            if(app.IsOpen) {
                app.Close();
            }
        }
    }
}
