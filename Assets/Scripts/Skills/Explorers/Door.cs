using UnityEngine;

public class Door : MonoBehaviour
{
    AvatarNetworkBehavior avatarNetwork;
    public GameObject player;
    public bool isOpen = false;

    public void Awake()
    {
        avatarNetwork = player.GetComponent<AvatarNetworkBehavior>();
    }

    //public void OpenDoor()
    //{
    //    GetComponent<Animator>().SetTrigger("Open");
    //}

    //public void CloseDoor()
    //{
    //    GetComponent<Animator>().SetTrigger("Close");
    //}

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
