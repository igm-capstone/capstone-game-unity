using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class SyncPosition : NetworkBehaviour
{

    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    private float lerpRate;
    private float normalLerpRate = 24;
    private float fasterLerpRate = 40;

    private Vector3 lastPos;
    private float threshold = 0.5f;

    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    private bool useHistoricalLerping = true;
    private float closeEnough = 0.20f;

    void Start()
    {
        lerpRate = normalLerpRate;
    }

    void Update()
    {
        LerpPosition();
    }

    void FixedUpdate()
    {
        TransmitPosition();
    }

    void LerpPosition()
    {
        if (hasAuthority) return;

        if (useHistoricalLerping)
        {
            HistoricalLerping();
        }
        else
        {
            OrdinaryLerping();
        }
    }


    void TransmitPosition()
    {
        if (!hasAuthority) return;
        if (Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        RpcSyncTransform(pos);
    }


    [ClientRpc]
    void RpcSyncTransform(Vector3 latestPos)
    {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }

    void OrdinaryLerping()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }

    void HistoricalLerping()
    {
        if (syncPosList.Count > 0)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);

            if (Vector3.Distance(myTransform.position, syncPosList[0]) < closeEnough)
            {
                syncPosList.RemoveAt(0);
            }

            if (syncPosList.Count > 10)
            {
                lerpRate = fasterLerpRate;
            }
            else
            {
                lerpRate = normalLerpRate;
            }
        }
    }
}
