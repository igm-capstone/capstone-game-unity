using UnityEngine;
using System.Collections;

public class MimicExplorer : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    Material[] MatArray;
    SkinnedMeshRenderer SknMshRend;

    void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        SknMshRend = GetComponentInChildren<SkinnedMeshRenderer>();

        // Check for the ghost and load the correct material.
        if (GameObject.Find("Me").GetComponent<GhostController>())
        {   // Load material for Ghost
            SknMshRend.sharedMaterial = MatArray[1];
        }
        else
        { // Load material for Explorers
            SknMshRend.sharedMaterial = MatArray[0];
        }
    }

    void SetAnimatorRunSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    void SetAnimatorAttack(bool state)
    {
        SetAnimatorTrigger("Attack", state);
    }

    void SetAnimatorConeAttack(bool state)
    {
        SetAnimatorTrigger("Attack", state);
    }

    void SetAnimatorLongAttack(bool state)
    {
        SetAnimatorTrigger("Attack", state);
    }

    void SetAnimatorHeal(bool state)
    {
        SetAnimatorTrigger("Attack", state);
    }

    void SetAnimatorAoE(bool state)
    {
        SetAnimatorTrigger("Attack", state);
    }

    void SetAnimatorDead(bool state)
    {
        SetAnimatorTrigger("Die", state);
    }

    void SetAnimatorTrigger(string trigger, bool state)
    {
        if (state)
        {
            animator.SetTrigger(trigger);
        }
        else
        {
            animator.ResetTrigger(trigger);
        }
    }

    public void move()
    {

    }
}
