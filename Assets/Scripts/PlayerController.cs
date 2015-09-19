using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    private Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private LayerMask moveableObjectLayer;
    [SerializeField]
    private float collisionRadius;
    private Collider2D[] moveableObstacle = new Collider2D[1];

	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () 
    {
        move();
        moveableObstacle = Physics2D.OverlapCircleAll(transform.position, collisionRadius, moveableObjectLayer);
        if (moveableObstacle[0].gameObject != null)
            moveObject();
	}

    private void move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(horizontal,vertical) * moveSpeed;
    }

    private void moveObject()
    {
        if(Input.GetKey(KeyCode.Space))
            moveableObstacle[0].transform.parent = gameObject.transform;
        else
            moveableObstacle[0].transform.parent = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, collisionRadius);
    }
}
