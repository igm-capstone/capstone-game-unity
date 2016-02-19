using UnityEngine;

public class Door : MonoBehaviour
{
    AvatarNetworkBehavior avatarNetwork;
    public GameObject player;
    public bool isOpen = false;

    public void SwingDoor()
    {
        if(isOpen)
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
