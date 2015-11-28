using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GhostNetworkBehavior : NetworkBehaviour {
    public NetworkConnection conn;

    public override void OnStartLocalPlayer () {
        GetComponent<GhostBehavior>().enabled = true;


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
