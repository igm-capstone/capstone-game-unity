using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RpcNetworkAnimator: NetworkBehaviour
{
    public Animator Animator;


    public void SetFloat(string name, float value)
    {
        if (isServer)
            RpcSetFloat(name, value);
        else
            CmdSetFloat(name,value);
        
    }

    public void SetFloat(string name, float value, float damp, float deltaTime)
    {
        RpcSetFloatDamp(name, value, damp, deltaTime);
    }

    public void SetInteger(string name, int value)
    {
        RpcSetInteger(name, value);
    }

    public void SetBool(string name, bool value)
    {
        if (isServer)
            RpcSetBool(name, value);
        else
            CmdSetBool(name,value);
        
    }

    public void SetTrigger(string name)
    {
        if (isServer)
            RpcSetTrigger(name);
        else
            CmdSetTrigger(name);
    }


    public void ResetTrigger(string name)
    {
        RpcResetTrigger(name);
    }


    [ClientRpc]
    void RpcSetFloat(string name, float value)
    {
        Animator.SetFloat(name, value);
    }
    [Command]
    void CmdSetFloat(string name, float value)
    {
        SetFloat(name, value);
    }

    [ClientRpc]
    void RpcSetFloatDamp(string name, float value, float damp, float deltaTime)
    {
        Animator.SetFloat(name, value, damp, deltaTime);
    }

    [ClientRpc]
    void RpcSetInteger(string name, int value)
    {
        Animator.SetInteger(name, value);
    }

    [ClientRpc]
    void RpcSetBool(string name, bool value)
    {
        Animator.SetBool(name, value);
    }

    [Command]
    void CmdSetBool(string name, bool value)
    {
        SetBool(name,value);
    }

    [ClientRpc]
    void RpcSetTrigger(string name)
    {
        Animator.SetTrigger(name);        
    }

    [Command]
    void CmdSetTrigger(string name)
    {
        SetTrigger(name);
    }

    [ClientRpc]
    void RpcResetTrigger(string name)
    {
        Animator.ResetTrigger(name);
    } 

}
