using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SkillBar))]
//[RequireComponent(typeof(MeleeWeaponBehavior))]
public class AvatarController : MonoBehaviour 
{
    [SerializeField]
    public float MoveSpeed = 4;
    [SerializeField]
    public bool Disabled { get { return (_health &&  _health.CurrentHealth <= 0); } }

    private Rigidbody2D _rb;
    private SkillBar _avatarSkillBar;
    private Health _health;
    private RpcNetworkAnimator animator;
    void Awake () {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();

        _avatarSkillBar = GetComponentInChildren<SkillBar>();
        animator = GetComponent<RpcNetworkAnimator>();
    }
    void Start()
    {
        _avatarSkillBar.enabled = true;
    }
    void FixedUpdate() 
    {
        if (!Disabled) 
            Move();
        else 
            HelpMessage.Instance.SetMessage("You are incapacitated. Seek help!");
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(vertical) > Mathf.Epsilon || Mathf.Abs(horizontal) > Mathf.Epsilon)
        {
            transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
        }
        _rb.velocity = new Vector2(horizontal,vertical) * MoveSpeed;
        animator.SetFloat("RunSpeed", _rb.velocity.magnitude);
    }

}
