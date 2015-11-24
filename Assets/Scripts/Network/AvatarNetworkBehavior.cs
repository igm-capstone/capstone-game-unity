using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AvatarNetworkBehavior : NetworkBehaviour {

	public override void OnStartLocalPlayer () {
        GetComponent<AvatarController>().enabled = true;
        GetComponent<MovementBroadcast>().enabled = true;
        transform.FindChild("Fog").GetComponent<SpriteRenderer>().color = Color.white;

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
