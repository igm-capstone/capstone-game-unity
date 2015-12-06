using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MeleeWeapon : ISkill
{
    public float SlashDuration = 0.2f;

    Transform weaponTransform;
    BoxCollider2D hitboxCollider;
    SpriteRenderer spriteRenderer;
    TrailRenderer trailRenderer;
    AvatarNetworkBehavior avatarNetwork;

    public void Awake()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        hitboxCollider = trailRenderer.gameObject.GetComponent<BoxCollider2D>();
        spriteRenderer = trailRenderer.gameObject.GetComponent<SpriteRenderer>();
        weaponTransform = trailRenderer.transform;
        avatarNetwork = GetComponentInParent<AvatarNetworkBehavior>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            Use();
        }
    }

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        StartCoroutine(Slash());
        return true;
    }

    public IEnumerator Slash()
    {
        avatarNetwork.CmdEnableSlash(true);
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
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Minion"))
        {
            collision.collider.GetComponent<MinionController>().CmdKill();
        }
    }


    public void EnableSlash(bool status)
    {
        hitboxCollider.enabled = status;
        trailRenderer.enabled = status;
        spriteRenderer.enabled = status;
    }


}
