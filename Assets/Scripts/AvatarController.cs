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
    public bool isHidden { get; set; }
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
    private Transform contanier;

    float slowDownMod;
    private float animSpeed;

    void Awake()
    {
        hauntEtoM = false;
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();

        _avatarSkillBar = GetComponentInChildren<SkillBar>();
        animator = GetComponent<RpcNetworkAnimator>();
        avatarNB = GetComponent<AvatarNetworkBehavior>();

        contanier = transform.Find("AvatarRotation");

        // Game Balance Adjust => Makes Speed number comparable to Minion Speed number.
        CalcMoveSpeed = MoveSpeed / SpeedAdjust;
        slowDownMod = 1.0f;
        isHidden = false;
    }

    void Start()
    {
        _avatarSkillBar.enabled = true;
    }

    void FixedUpdate()
    {
        CalcMoveSpeed = MoveSpeed / SpeedAdjust;
        if (Disabled || isHidden)
        {
            _rb.velocity = new Vector2(0, 0);
            if(Disabled)
                HelpMessage.Instance.SetMessage("You are incapacitated. Seek help!");
        }
        else
        {
            Move();
        }
    }


    Vector2 currentSpeed;
    float currAngle;

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float hSpeed = horizontal > 0 ? 1 : horizontal < 0 ? -1 : 0;
        float vSpeed = vertical > 0 ? 1 : vertical < 0 ? -1 : 0;

        if (isAttacking == false)
        {

            bool wantToMove = hSpeed != 0 || vSpeed != 0;

            var dirVector = wantToMove ? new Vector2(hSpeed, vSpeed).normalized : new Vector2();

            // speed per second
            var targetSpeed = wantToMove ? CalcMoveSpeed * slowDownMod * dirVector : new Vector2();

            var mAcceleration = 10f;
            currentSpeed = Vector2.Lerp(currentSpeed, targetSpeed, mAcceleration * Time.deltaTime);

            // delta space for the current frame
            _rb.velocity = currentSpeed;

            if (wantToMove)
            {
                float targetAngle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
                currAngle = Mathf.LerpAngle(currAngle, targetAngle, 8 * Time.deltaTime);
                contanier.rotation = Quaternion.AngleAxis(currAngle, Vector3.forward);
            }

            //// Keyboard only controls - Rotation
            //if (Mathf.Abs(vertical) > Mathf.Epsilon || Mathf.Abs(horizontal) > Mathf.Epsilon)
            //{
            //    transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
            //}

            //// Applies velocity
            //_rb.velocity = new Vector2(horizontal, vertical).normalized * CalcMoveSpeed * slowDownMod*
            //    (Mathf.Abs(Mathf.Abs(horizontal) - Mathf.Abs(vertical)) + (Mathf.Abs(horizontal) * Mathf.Abs(vertical) * Mathf.Sqrt(2)));
        }
        else
        {
            _rb.velocity = new Vector2(0.0f, 0.0f);
        }

        var speed = _rb.velocity.magnitude;
        if (animSpeed != speed)
        {
            animator.SetFloat("RunSpeed", speed);
            //if (hauntEtoM)
            //{
            //    //Debug.Log("update minion animator");
            //    //hauntedMinionAnimator.SetFloat("Speed", _rb.velocity.magnitude);
            //}
            //BroadcastMessage("SetAnimatorSpeed", speed, SendMessageOptions.DontRequireReceiver);
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
        animator.Broadcast = hauntMinionActive;

        hauntMinionToControl = minion;
        if(hauntMinionToControl != null)
        {
            hauntedMinionAnimator = hauntMinionToControl.GetComponentInChildren<RpcNetworkAnimator>();
        }
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

    public void Message(string str)
    {
        if (gameObject.name == "Me")
        {
            HelpMessage.Instance.SetMessage(str);
        }
    }
}
