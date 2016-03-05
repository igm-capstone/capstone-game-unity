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

    public void Start()
    {
        if (interactUI == null)
        {
            var ui = transform.FindChild("InteractUI");
            if (ui)
            {
                interactUI = ui.gameObject;
            }
        }

        playerOnTrigger = 0;
    }


    // This will be used to implement Interactable Objects UI!
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        playerOnTrigger++;
            
        if (!interactUI)
        {
            return;
        }


        var ac = GetComponentInParent<AvatarController>();
        // only show interact ui for desabled players
        if (Type == InteractableObject.Player && !ac.Disabled)
        {
            return;
        }
        
        interactUI.SetActive(true);
        interactUI.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        playerOnTrigger--;
            
        // Hardfix for unexpedted nunmbers.
        if (playerOnTrigger <0)
            playerOnTrigger = 0;

        if (interactUI && playerOnTrigger == 0)
        {
            interactUI.SetActive(false);
        }

    }
}
