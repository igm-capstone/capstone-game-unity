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
        RpcSetFloat(name, value);
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
        RpcSetBool(name, value);
    }

    public void SetTrigger(string name)
    {
        RpcSetTrigger(name);
    }


    public void ResetTrigger(string name)
    {
        ResetTrigger(name);
    }


    [ClientRpc]
    void RpcSetFloat(string name, float value)
    {
        Animator.SetFloat(name, value);
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

    [ClientRpc]
    void RpcSetTrigger(string name)
    {
        Animator.SetTrigger(name);
    }

    [ClientRpc]
    void RpcResetTrigger(string name)
    {
        Animator.ResetTrigger(name);
    } 

}
