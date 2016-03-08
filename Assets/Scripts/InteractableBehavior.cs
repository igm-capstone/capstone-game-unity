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
    AvatarController ac;

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
        if (Type == InteractableObject.Player)
        {
            Debug.Log("interact " + name);
            ac = GetComponentInParent<AvatarController>();
        }
    }


    void Update()
    {
        if (Type == InteractableObject.Player)
        {
            //Debug.Log("up " + name);
            interactUI.SetActive(ac.Disabled);
            interactUI.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
    }


    // This will be used to implement Interactable Objects UI!
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        if (other.name != "Me")
        {
            return;
        }

        playerOnTrigger++;
            
        if (other.name == "Me" && interactUI)
        {
            interactUI.SetActive(true);
            interactUI.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
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

        if (other.name == "Me" && interactUI && playerOnTrigger == 0)
        {
            interactUI.SetActive(false);
        }

    }
}
