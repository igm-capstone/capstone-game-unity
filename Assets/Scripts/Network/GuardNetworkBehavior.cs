using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GuardNetworkBehavior : NetworkBehaviour {
    
    public override void OnStartLocalPlayer () {
        GetComponent<GuardBehavior>().enabled = true;
	}

}
