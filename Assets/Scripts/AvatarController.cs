using UnityEngine;
using UnityEngine.Networking;

public class AvatarController : MonoBehaviour 
{
    private Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private LayerMask moveableObjectLayer;
    [SerializeField]
    private float collisionRadius;
    public Collider2D[] moveableObstacle = new Collider2D[1];
    private bool disable = false;
    private Transform blockCollector;

    MeleeWeaponBehavior weaponBehavior;

	void Start () {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        weaponBehavior = GetComponentInChildren<MeleeWeaponBehavior>();
        blockCollector = GameObject.Find("BlocksCollector").transform;
	}
	
	void FixedUpdate () 
    {
        if(!disable)
            move();
        moveableObstacle = Physics2D.OverlapCircleAll(transform.position, collisionRadius, moveableObjectLayer);
        if (moveableObstacle.Length > 0 && moveableObstacle[0].gameObject != null)
            moveObject();

        //Test Inputs
        // Restart the Level
        if(Input.GetKey(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        // Close Application
        if ((Input.GetKey(KeyCode.F4))&&(Input.GetKey(KeyCode.RightAlt)|| Input.GetKey(KeyCode.LeftAlt)))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.M))
        {
            weaponBehavior.StartCoroutine(weaponBehavior.Slash());
        }
    }

    private void move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
        rb.velocity = new Vector2(horizontal,vertical) * moveSpeed;
    }

    private void moveObject()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (moveableObstacle[0].transform.parent == blockCollector)
            {
                if (moveableObstacle[0].transform.parent != gameObject.transform.parent.transform)
                {
                    moveableObstacle[0].transform.parent = gameObject.transform.parent.transform;
                    transform.parent.GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, true);
                }
            }
        }
        else
        {
            if (moveableObstacle[0].transform.parent == gameObject.transform.parent.transform)
            {
                moveableObstacle[0].transform.parent = blockCollector;
                transform.parent.GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, false);
            }
        }
    }

    private void MeleeAttack()
    {

    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, collisionRadius);
    }
    #endif

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            transform.parent.GetComponent<AvatarNetworkBehavior>().CmdEndGame("Prisoner Wins!");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && 
             ( collision.contacts[0].collider.gameObject.layer == LayerMask.NameToLayer("Player") ||
                collision.contacts[0].otherCollider.gameObject.layer == LayerMask.NameToLayer("Player") ) )
        {
            disable = true;
            transform.parent.GetComponent<AvatarNetworkBehavior>().CmdEndGame("Guard Wins!");
        }
        else if((collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
                 (collision.contacts[0].collider.gameObject.layer == LayerMask.NameToLayer("Player") ||
                    collision.contacts[0].otherCollider.gameObject.layer == LayerMask.NameToLayer("Player"))))
        {
            disable = false;
        }
    }

}
