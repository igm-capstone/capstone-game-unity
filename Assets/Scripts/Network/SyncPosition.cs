using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class SyncPosition : NetworkBehaviour
{
    struct SyncPoint
    {
        public Vector3 position;
        public float timestamp;
    }

    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    private float lerpRate;
    private float normalLerpRate = 24;
    private float fasterLerpRate = 40;

    private Vector3 lastPos;
    private float threshold = 0.5f;

    private List<SyncPoint> syncPosList = new List<SyncPoint>();

    [SerializeField]
    private bool useHistoricalLerping = true;

    [Range(0, 2), SerializeField]
    private float historicalLerpingTimeout = 1;

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

        if (useHistoricalLerping)
        {
            syncPosList.Add(new SyncPoint { position = syncPos, timestamp = Time.time });
        }
    }

    void OrdinaryLerping()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }

    void HistoricalLerping()
    {
        if (syncPosList.Count > 0)
        {
            var point = syncPosList[0];

            if (Time.time - point.timestamp > historicalLerpingTimeout)
            {
                // force next position
                // whenever taking too much to get to it.
                myTransform.position = point.position;
            }

            myTransform.position = Vector3.Lerp(myTransform.position, point.position, Time.deltaTime * lerpRate);

            if (Vector3.Distance(myTransform.position, point.position) < closeEnough)
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
