using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class GrenadeToss : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;

    // Class Variables
    public float ExploRadius;
    public GameObject ExploPreFab;
    Vector3 ExploPos;
    public LayerMask HitLayers;

    public float MaxThrowRange = 5.0f;

    public int Damage = 2;
    public bool hasKnockBack = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;


    Vector3 mouseWorldPos;

    public void Awake()
    {
        Name = "GrenadeToss";
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();


        // Set key code:
        key = KeyCode.Mouse0;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }

        if (IsReady())
        {
            GetComponent<AvatarController>().isAttacking = false;
        }
        

        // Get mouse position in World Coordinates
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
            GridBehavior.Instance.getNodeAtPos(transform.position).ZIndex);
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Usar Screen to ray.

        // Calculate Explosion position
        ExploPos = (mouseWorldPos - transform.position);


        //Debug.DrawRay(transform.position, ExploPos, Color.yellow);

        Debug.DrawLine(Input.mousePosition, mouseScreenPos, Color.blue);
        Debug.DrawLine(mouseScreenPos, mouseWorldPos, Color.yellow);
        Debug.DrawLine(mouseWorldPos, ExploPos,Color.red);
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        // Turns to mouse position.
        TurnToMousePos();




        // Trigger animation
        animator.SetTrigger("LongAttack");
        GetComponent<AvatarController>().isAttacking = true;

        // Instantiate Explosion
        Instantiate(ExploPreFab, ExploPos, Quaternion.identity);

        // Get targets
        var TargetsHit = Physics2D.OverlapCircleAll(ExploPos, ExploRadius, HitLayers);

        foreach (var trgt in TargetsHit)
        {
            // Check if knockBack is enabled and if hitting a minion.
            if (hasKnockBack && trgt.gameObject.layer == LayerMask.NameToLayer("Minion"))
            {
                avatarNetwork.CmdAssignDamageWithForce(trgt.gameObject, Damage, KnockBackMag);
            }
            else
            {
                avatarNetwork.CmdAssignDamage(trgt.gameObject, Damage);
            }
        } //foreach

        return null;
    }
}