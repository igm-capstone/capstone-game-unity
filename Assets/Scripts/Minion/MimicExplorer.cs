using UnityEngine;
using System.Collections;

public class MimicExplorer : MonoBehaviour
{
    Animator animator;

    void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {

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
