using UnityEngine;
using System.Collections;

public class PlantBehavior : MonoBehaviour
{
    public float Damage;
    bool hasTarget;
    GameObject AtckTarget;

    // Use this for initialization
    void Start()
    {
        hasTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            MeeleeAttack(AtckTarget);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && hasTarget == false)
        {
            AtckTarget = other.gameObject;
            hasTarget = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject == AtckTarget)
        {
            hasTarget = false;

            // Turn to Right
            Vector2 lookDir = transform.right;
            transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);
        }
    }

    void MeeleeAttack(GameObject Target)
    {
        // Turn to Target
        Vector2 lookDir = Target.transform.position - transform.position;
        transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);
    }
}
