using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeApp : App
{
    protected override void Update () {
        // doesn't do anything except not call base
        // to prevent slide animation
    }

    public override void Open() {
        base.Open();
    }

    public override void Close() { 
        base.Close();
    }
}
