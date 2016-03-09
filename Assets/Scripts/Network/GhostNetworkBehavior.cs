using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GhostNetworkBehavior : BasePlayerNetworkBehavior {
    public override void OnStartLocalPlayer () {
        GetComponent<GhostController>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
        GetComponentInChildren<AudioSource>().enabled = true;

        base.OnStartLocalPlayer();
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(clickPos, Vector2.zero,1000, 1 << LayerMask.NameToLayer("Door"));

            if (hitInfo && hitInfo.collider.gameObject.CompareTag("Door"))
            {
                CmdOpenDoor(hitInfo.collider.transform.parent.gameObject);
            }
        }
    }

    [Command]
    void CmdOpenDoor(GameObject obj)
    {
        if (!obj.GetComponent<Door>().isSwinging)
        {
            RpcOpenDoor(obj);
        }
    }

    [ClientRpc]
    void RpcOpenDoor(GameObject obj)
    {
        obj.GetComponent<Door>().SwingDoor();
    }
}
