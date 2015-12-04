using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GhostNetworkBehavior : BasePlayerNetworkBehavior {
    public override void OnStartLocalPlayer () {
        GetComponent<GhostController>().enabled = true;
        
        base.OnStartLocalPlayer();
    }

}
