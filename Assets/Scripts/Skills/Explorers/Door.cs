using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    public bool canOpen = true;
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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
                isOpen = false;
                GetComponent<PolygonCollider2D>().enabled = true;
                GridBehavior.Instance.SetGridDirty();
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
}
