using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SkillBar))]
//[RequireComponent(typeof(MeleeWeaponBehavior))]
public class AvatarController : MonoBehaviour 
{
    [SerializeField]
    public float MoveSpeed = 4;
    public float CalcMoveSpeed;
    float SpeedAdjust = 10;
    [SerializeField]
    public bool Disabled { get { return (_health &&  _health.CurrentHealth <= 0); } }

    private Rigidbody2D _rb;
    private SkillBar _avatarSkillBar;
    private Health _health;
    private RpcNetworkAnimator animator;
    private RpcNetworkAnimator hauntedMinionAnimator;
    private GameObject hauntMinionToControl;

    public bool isAttacking;
    public bool hauntEtoM { get; set; }

    void Awake ()
    {
        hauntEtoM = false;
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();

        _avatarSkillBar = GetComponentInChildren<SkillBar>();
        animator = GetComponent<RpcNetworkAnimator>();

        // Game Balance Adjust => Makes Speed number comparable to Minion Speed number.
        CalcMoveSpeed = MoveSpeed / SpeedAdjust;
    }

    void Start()
    {
        _avatarSkillBar.enabled = true;
    }

    void FixedUpdate() 
    {
        CalcMoveSpeed = MoveSpeed / SpeedAdjust;
        if (!Disabled) 
            Move();
        else
        {
            _rb.velocity = new Vector2(0, 0);
            HelpMessage.Instance.SetMessage("You are incapacitated. Seek help!");
        }   
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        /*
        // Mouse and Key controls - Rotation
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        float LookPosX = mouseWorldPos.x - transform.position.x;
        float LookPosY = mouseWorldPos.y - transform.position.y;
        transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(LookPosY, LookPosX) * Mathf.Rad2Deg, Vector3.forward);
        */

        if (isAttacking == false)
        {
            // Keyboard only controls - Rotation
            if (Mathf.Abs(vertical) > Mathf.Epsilon || Mathf.Abs(horizontal) > Mathf.Epsilon)
            {
                transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
            }

            // Applies velocity
            _rb.velocity = new Vector2(horizontal, vertical).normalized * CalcMoveSpeed*
                (Mathf.Abs(Mathf.Abs(horizontal) - Mathf.Abs(vertical)) + (Mathf.Abs(horizontal) * Mathf.Abs(vertical) * Mathf.Sqrt(2)));
            animator.SetFloat("RunSpeed", _rb.velocity.magnitude);
            if (hauntEtoM)
            {
                hauntedMinionAnimator.SetFloat("Speed", _rb.velocity.magnitude);
            }


        }
        else
        {
            _rb.velocity = new Vector2(0.0f, 0.0f);
            animator.SetFloat("RunSpeed", _rb.velocity.magnitude);
        }

        var pos = transform.position;
        var nodee = GridBehavior.Instance.getNodeAtPos(pos);
        if (nodee != null)
        {
            pos.z = nodee.ZIndex;
            transform.position = pos;
        }

    }

    public void SetMinionToControl(GameObject minion)
    {
        hauntEtoM = true;
        hauntMinionToControl = minion;
        hauntedMinionAnimator = hauntMinionToControl.GetComponent<RpcNetworkAnimator>();
    }
} 
