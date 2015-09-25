using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PrisonerNetworkBehavior : NetworkBehaviour {

	public override void OnStartLocalPlayer () {
        GetComponent<PlayerController>().enabled = true;
        GetComponent<MovementBroadcast>().enabled = true;
	}
}
