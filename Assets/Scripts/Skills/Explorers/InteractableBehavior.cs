using UnityEngine;
using System.Collections;

public enum InteractableObject
{
    Door,
    Player,
    Light,
    HidingSpot,
    Goal
}

public class InteractableBehavior : MonoBehaviour
{
    public InteractableObject Type;

    int playerOnTrigger;

    public GameObject interactUI;
    GameObject topmostParent;

    public void Start()
    {
        if (interactUI == null)
        {
            interactUI = transform.FindChild("InteractUI").gameObject;
        }

        topmostParent = this.transform.root.gameObject;

        playerOnTrigger = 0;
    }


    // This will be used to implement Interactable Objects UI!
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerOnTrigger++;
            
            // Check if this is a player and if the other player is still active, or if this is anything else.
            if ((Type == InteractableObject.Player && other.transform.root.GetComponent<AvatarController>().Disabled)
                || Type != InteractableObject.Player)
            {
                interactUI.SetActive(true);
                interactUI.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerOnTrigger--;
            
            // Hardfix for unexpedted nunmbers.
            if (playerOnTrigger <0)
                playerOnTrigger = 0;

            if (playerOnTrigger == 0)
                interactUI.SetActive(false);
        }

    }
}
