using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RpcNetworkAnimator: NetworkBehaviour
{
    public Animator Animator;

    [SyncVar]
    public bool Broadcast;


    public void SetFloat(string name, float value)
    {
        if (isServer) RpcSetFloat(name, value);
        else CmdSetFloat(name, value);
    }

    public void SetFloat(string name, float value, float damp, float deltaTime)
    {
        if (isServer) RpcSetFloatDamp(name, value, damp, deltaTime);
        else CmdSetFloatDamp(name, value, damp, deltaTime);
    }

    public void SetInteger(string name, int value)
    {
        if (isServer) RpcSetInteger(name, value);
        else CmdSetInteger(name, value);
    }

    public void SetBool(string name, bool value)
    {
        if (isServer) RpcSetBool(name, value);
        else CmdSetBool(name, value);
    }

    public void SetTrigger(string name)
    {
        if (isServer) RpcSetTrigger(name);
        else CmdSetTrigger(name);
    }

    public void ResetTrigger(string name)
    {
        if (isServer) RpcResetTrigger(name);
        else CmdResetTrigger(name);
    }

    [Command]
    void CmdSetFloat(string name, float value)
    {
        RpcSetFloat(name, value);
    }

    [Command]
    void CmdSetFloatDamp(string name, float value, float damp, float deltaTime)
    {
        RpcSetFloatDamp(name, value, damp, deltaTime);
    }

    [Command]
    void CmdSetInteger(string name, int value)
    {
        RpcSetInteger(name, value);
    }

    [Command]
    void CmdSetBool(string name, bool value)
    {
        RpcSetBool(name, value);
    }

    [Command]
    void CmdSetTrigger(string name)
    {
        RpcSetTrigger(name);
    }

    [Command]
    void CmdResetTrigger(string name)
    {
        RpcResetTrigger(name);
    }



    [ClientRpc]
    void RpcSetFloat(string name, float value)
    {
        if (Animator.isInitialized)
        {
            Animator.SetFloat(name, value);
        }

        if (Broadcast)
        {
            BroadcastMessage("SetAnimator" + name, value, SendMessageOptions.DontRequireReceiver);
        }
    }

    [ClientRpc]
    void RpcSetFloatDamp(string name, float value, float damp, float deltaTime)
    {
        if (Animator.isInitialized)
        {
            Animator.SetFloat(name, value, damp, deltaTime);
        }

        if (Broadcast)
        {
            BroadcastMessage("SetAnimator" + name, value, SendMessageOptions.DontRequireReceiver);
        }
    }

    [ClientRpc]
    void RpcSetInteger(string name, int value)
    {
        if (Animator.isInitialized)
        {
            Animator.SetInteger(name, value);
        }

        if (Broadcast)
        {
            BroadcastMessage("SetAnimator" + name, value, SendMessageOptions.DontRequireReceiver);
        }
    }

    [ClientRpc]
    void RpcSetBool(string name, bool value)
    {
        if (Animator.isInitialized)
        {
            Animator.SetBool(name, value);
        }

        if (Broadcast)
        {
            BroadcastMessage("SetAnimator" + name, value, SendMessageOptions.DontRequireReceiver);
        }
    }

    [ClientRpc]
    void RpcSetTrigger(string name)
    {
        if (Animator.isInitialized)
        {
            Animator.SetTrigger(name);
        }

        if (Broadcast)
        {
            BroadcastMessage("SetAnimator" + name, true, SendMessageOptions.DontRequireReceiver);
        }
    }

    [ClientRpc]
    void RpcResetTrigger(string name)
    {
        if (Animator.isInitialized)
        {
            Animator.ResetTrigger(name);
        }

        if (Broadcast)
        {
            BroadcastMessage("SetAnimator" + name, false, SendMessageOptions.DontRequireReceiver);
        }
    } 

}
