using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MinionController))]
public class MinionBehaviour : MonoBehaviour
{
    private MinionController _controller;


    protected MinionController Controller
    {
        get { return _controller = _controller ?? GetComponent<MinionController>(); }
    }

    public virtual void UpdateBehaviour()
    {

    }

    public virtual void ActivateBehaviour()
    {

    }

    public virtual void DeactivateBehaviour()
    {

    }
}
