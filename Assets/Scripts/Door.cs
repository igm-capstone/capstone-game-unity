﻿using System.Linq;
using UnityEngine;

[Rig3DAsset("doors", Rig3DExports.Position | Rig3DExports.Rotation | Rig3DExports.Scale)]
public class Door : MonoBehaviour
{
    public bool isOpen = false;

    [Export]
    public bool canOpen = true;

    public bool isSwinging { get; private set; }

    [Export]
    public Bounds bounds
    {
        get
        {
            Bounds localBounds = transform.Find("Mesh").GetComponent<MeshFilter>().sharedMesh.bounds;
            Bounds bounds = new Bounds(transform.TransformPoint(localBounds.min), Vector3.zero);
            bounds.Encapsulate(transform.TransformPoint(localBounds.max));
            return bounds;
        }
    }

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (!canOpen)
        {
            GetComponentInChildren<InteractableBehavior>().enabled = false;
            GetComponentInChildren<BoxCollider2D>().enabled = false;
        }
    }

    public void SwingDoor()
    {
        if(!canOpen)
        {
            //UI message - can't open door
            //Debug.Log("Door Broken");
        }
        else
        {
            //Debug.Log("door anim start");
            isSwinging = true;

            if (isOpen)
            {
                animator.SetTrigger("Close");
            }
            else
            {
                animator.SetTrigger("Open");
                isOpen = true;
                GetComponent<PolygonCollider2D>().enabled = false;
                GridBehavior.Instance.SetGridDirty();
            }
        }
        
    }

    void SwingOpenAnimationComplete()
    {
        Debug.Log("door anim complete");
        isSwinging = false;
    }

    void SwingCloseAnimationComplete()
    {
        Debug.Log("door anim complete");
        isSwinging = false;

        isOpen = false;
        GetComponent<PolygonCollider2D>().enabled = true;
        GridBehavior.Instance.SetGridDirty();
    }
}
