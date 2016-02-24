using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    AvatarNetworkBehavior avatarNetwork;
    public bool isOpen = false;
    public bool canOpen = true;
    private Collider2D collider2D;
    private GameObject doorMesh;
    private RpcNetworkAnimator animator;

    void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        doorMesh = transform.FindChild("Mesh").gameObject;
        animator = GetComponentInChildren<RpcNetworkAnimator>();
    }

    bool f(Collider2D c)
    {
        return !c.isTrigger;
    }

    public void SwingDoor()
    {
        if(!canOpen)
        {
            //UI message - can't open door
            Debug.Log("Door Broken");
        }
        else
        {
            if (isOpen)
            {
                animator.SetTrigger("Close");
                collider2D.enabled = true;
                isOpen = false;
                GridBehavior.Instance.SetGridDirty();
            }
            else
            {
                animator.SetTrigger("Open");
                collider2D.enabled = false;
                isOpen = true;
                GridBehavior.Instance.SetGridDirty();
            }
        }
        
    }
}
