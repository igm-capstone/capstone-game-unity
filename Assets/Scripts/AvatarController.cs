using UnityEngine;
using System.Collections;
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
    public bool Disabled { get { return (_health && _health.CurrentHealth <= 0); } }
    public bool isAttacking;
    public bool hauntEtoM { get; set; }

    private Rigidbody2D _rb;
    private SkillBar _avatarSkillBar;
    private Health _health;
    private RpcNetworkAnimator animator;
    private RpcNetworkAnimator hauntedMinionAnimator;
    private GameObject hauntMinionToControl;
    private GameObject doorToOpen;
    private AvatarNetworkBehavior avatarNB;

    float slowDownMod;
    void Awake()
    {
        hauntEtoM = false;
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();

        _avatarSkillBar = GetComponentInChildren<SkillBar>();
        animator = GetComponent<RpcNetworkAnimator>();
        avatarNB = GetComponent<AvatarNetworkBehavior>();

        // Game Balance Adjust => Makes Speed number comparable to Minion Speed number.
        CalcMoveSpeed = MoveSpeed / SpeedAdjust;
        slowDownMod = 1.0f;
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

        if (isAttacking == false)
        {
            // Keyboard only controls - Rotation
            if (Mathf.Abs(vertical) > Mathf.Epsilon || Mathf.Abs(horizontal) > Mathf.Epsilon)
            {
                transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
            }

            // Applies velocity
            _rb.velocity = new Vector2(horizontal, vertical).normalized * CalcMoveSpeed * slowDownMod*
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
            pos.z = Mathf.Lerp(pos.z, nodee.ZIndex, Time.deltaTime * 10);
            transform.position = pos;
        }

    }

    public void SetMinionToControl(GameObject minion, bool hauntMinionActive)
    {
        hauntEtoM = hauntMinionActive;
        hauntMinionToControl = minion;
        if(hauntMinionToControl != null)
            hauntedMinionAnimator = hauntMinionToControl.GetComponent<RpcNetworkAnimator>();
    }

    // Functions to apply slowdown on player
    public void StartSlowDown(float _slowRate)
    {
        slowDownMod = _slowRate;
    }

    public void StopSlowDown()
    {
        slowDownMod = 1.0f;
    }
}
