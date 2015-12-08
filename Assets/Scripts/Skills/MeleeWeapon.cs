using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MeleeWeapon : ISkill
{
    public float SlashDuration = 0.3f;
    public int Damage = 1;

    Transform weaponTransform;
    Collider2D hitboxCollider;
    SpriteRenderer spriteRenderer;
    TrailRenderer trailRenderer;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;

    public void Awake()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        hitboxCollider = trailRenderer.gameObject.GetComponent<Collider2D>();
        spriteRenderer = trailRenderer.gameObject.GetComponent<SpriteRenderer>();
        weaponTransform = trailRenderer.transform;
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        
        StartCoroutine(Slash());
        return null;
    }

    public IEnumerator Slash()
    {
        avatarNetwork.CmdEnableSlash(true);
        hitboxCollider.enabled = true;
        Quaternion q0 = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        Quaternion q1 = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        float time = 0.0f;

        while (time < SlashDuration)
        {
            weaponTransform.parent.localRotation = Quaternion.Slerp(q0, q1, time / SlashDuration);
            time += Time.deltaTime;
            yield return null;
        }

        weaponTransform.parent.localRotation = Quaternion.identity;
        avatarNetwork.CmdEnableSlash(false);
        hitboxCollider.enabled = false;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (hitboxCollider.enabled) //Very important!
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Minion"))
            {
                avatarNetwork.CmdAssignDamage(other.gameObject, Damage);
                hitboxCollider.enabled = false;  //Very important!
            }
        }
    }


    public void EnableSlash(bool status)
    {
        trailRenderer.enabled = status;
        spriteRenderer.enabled = status;
    }


}
