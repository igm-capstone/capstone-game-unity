using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class SyncRotationChild : NetworkBehaviour
{

    private float syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;
    private float lerpRate = 20;

    private float lastPlayerRot;
    private float threshold = 5.0f;

    private List<float> syncPlayerRotList = new List<float>();
    private float closeEnough = 10.0f;
    [SerializeField]
    private bool useHistoricalInterpolation;

    void Start()
    {

    }

    void Update()
    {
        LerpRotations();
    }

    void FixedUpdate()
    {
        TransmitRotations();
    }

    void LerpRotations()
    {
        if (hasAuthority) return;
        if (useHistoricalInterpolation)
        {
            HistoricalInterpolation();
        }
        else
        {
            OrdinaryLerping();
        }
    }

    void TransmitRotations()
    {
        if (!hasAuthority) return;
        if ((Mathf.Abs(playerTransform.localEulerAngles.z - lastPlayerRot) > threshold))
        {
            CmdProvidePositionToServer(playerTransform.localEulerAngles.z);
            lastPlayerRot = playerTransform.localEulerAngles.z;
            Debug.Log("Command tcp");
        }
    }

    [Command]
    void CmdProvidePositionToServer(float angle)
    {
        RpcSyncRotation(angle);
        Debug.Log("Command rcv");
    }


    [ClientRpc]

    void RpcSyncRotation(float latestPlayerRot)
    {
        syncPlayerRotation = latestPlayerRot;
        syncPlayerRotList.Add(syncPlayerRotation);
        Debug.Log("Rpc rcv");
    }

    void HistoricalInterpolation()
    {
        if (syncPlayerRotList.Count > 0)
        {
            Vector3 playerNewRot = new Vector3(0, 0, syncPlayerRotList[0]);
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNewRot), lerpRate * Time.deltaTime);

            if (Mathf.Abs(playerTransform.localEulerAngles.z - syncPlayerRotList[0]) < closeEnough)
            {
                syncPlayerRotList.RemoveAt(0);
            }
        }
    }

    void OrdinaryLerping()
    {
        Vector3 playerNewRot = new Vector3(0, 0, syncPlayerRotation);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNewRot), lerpRate * Time.deltaTime);

    }
}