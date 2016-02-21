using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    AvatarNetworkBehavior avatarNetwork;
    public GameObject player;
    private bool isOpen = false;
    public bool canOpen = true;
    private Collider2D collider2D;

    void Awake()
    {
        collider2D = transform.Find("ActualCollider").GetComponent<Collider2D>();
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
                GetComponent<Animator>().SetTrigger("Close");
                collider2D.enabled = true;
                isOpen = false;
                GridBehavior.Instance.SetGridDirty();
            }
            else
            {
                GetComponent<Animator>().SetTrigger("Open");
                collider2D.enabled = false;
                isOpen = true;
                GridBehavior.Instance.SetGridDirty();
            }
        }
        
    }
}
