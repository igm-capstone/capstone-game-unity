﻿using UnityEngine;
using System.Collections;

public class MeleeWeaponBehavior : MonoBehaviour
{
    public float SlashDuration = 0.2f;

    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator Slash()
    {
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;

        Quaternion q0 = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        Quaternion q1 = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        float time = 0.0f;

        while (time < SlashDuration)
        {
            transform.parent.localRotation = Quaternion.Slerp(q0, q1, time / SlashDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.parent.localRotation = Quaternion.identity;
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("APPLY DAMAGE LOGIC HERE");
        }
    }
}