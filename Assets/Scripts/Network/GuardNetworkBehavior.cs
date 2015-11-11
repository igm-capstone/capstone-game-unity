using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GuardNetworkBehavior : NetworkBehaviour {
    
    public override void OnStartLocalPlayer () {
        GetComponent<GuardBehavior>().enabled = true;


        foreach (var light in FindObjectsOfType<LightController>())
        {
            light.dirty = true;
        }

        foreach (var grid in FindObjectsOfType<GridBehavior>())
        {
            grid.SetGridDirty();
            grid.SetAIDirty();
        }
	}

}
