using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SkillBar))]
//[RequireComponent(typeof(MeleeWeaponBehavior))]
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
    public bool disable = false;

    SkillBar _avatarSkillBar;

    void Start () {
        _avatarSkillBar = GetComponentInChildren<SkillBar>();
        _avatarSkillBar.enabled = true;

        rb = transform.parent.GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () 
    {
        if(!disable)
            move();
        moveableObstacle = Physics2D.OverlapCircleAll(transform.position, collisionRadius, moveableObjectLayer);
        if (moveableObstacle.Length > 0 && moveableObstacle[0].gameObject != null)
            moveObject();

    }

    private void move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(vertical) > Mathf.Epsilon || Mathf.Abs(horizontal) > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
        }

        rb.velocity = new Vector2(horizontal,vertical) * moveSpeed;
    }

    private void moveObject()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.parent.GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, true);
        }
        else
        {
            transform.parent.GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, false);
        }
    }

}
