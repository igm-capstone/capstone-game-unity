using UnityEngine;

public class Door : MonoBehaviour
{
    AvatarNetworkBehavior avatarNetwork;
    public GameObject player;
    private bool isOpen = false;
    public bool canOpen = true;

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
                isOpen = false;
            }
            else
            {
                GetComponent<Animator>().SetTrigger("Open");
                isOpen = true;
            }
        }
        
    }
}
