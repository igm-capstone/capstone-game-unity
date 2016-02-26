using UnityEngine;
using System.Collections;

public enum InteractableObject
{
    Door,
    Player,
    Light,
    Goal
}

public class InteractableBehavior : MonoBehaviour
{
    public InteractableObject Type;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
    // This will be used to implement Interactable Objects UI!
    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("I am hitting the player!");
        }

        Debug.Log("I am hitting something else: " + other.gameObject.layer);
    }
}
