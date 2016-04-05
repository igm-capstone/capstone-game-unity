using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

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

    [ClientRpc]
    public void RpcDestroyHealthBar()
    {
        StartCoroutine(RemoveExplorerHealth());
    }

    IEnumerator RemoveExplorerHealth()
    {
        yield return null;

        List<GameObject> avatarCtrl = FindObjectsOfType<AvatarController>().Select(a => a.GetComponent<Health>().canvas).ToList();

        List<GameObject> allExplorersHealth = GameObject.FindGameObjectsWithTag("ExplorerHealth").ToList();

        for (int i = allExplorersHealth.Count - 1; i >= 0; i--)
        {
            if (avatarCtrl.Contains(allExplorersHealth[i]))
            {
                allExplorersHealth.RemoveAt(i);
            }

        }

        foreach (var item in allExplorersHealth)
        {
            Destroy(item);
        }
    }
}
