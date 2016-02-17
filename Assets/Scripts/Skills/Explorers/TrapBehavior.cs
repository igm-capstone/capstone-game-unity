using UnityEngine;
using System.Collections;

public class TrapBehavior : MonoBehaviour {

    CircleCollider2D TriggerCol;
    ParticleSystem PSys;

    public TrapType Type;

    public float TriggerRadius;
    public float TrapDuration;
    public float EffectTime;

    bool isTrapActive;
    bool mustApplyEffect;

	// Use this for initialization
	void Awake ()
    {
        // Component Getters
        TriggerCol = GetComponentInChildren<CircleCollider2D>();
        TriggerCol.radius = TriggerRadius;

        if (Type == TrapType.Poison)
        {
            PSys = GetComponent<ParticleSystem>();
            PSys.Stop();
        }

        // Starting Values
        isTrapActive = false;
        mustApplyEffect = false;

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Minion"))
        {
            // spring the Trap
            StartCoroutine(EffectTimer(EffectTime));
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (mustApplyEffect)
        {

        }
    }

    // Wait for Trap Effect Time to apply the Trap effect.
    IEnumerator EffectTimer(float _EffectTime)
    {
        yield return new WaitForSeconds(_EffectTime);
        mustApplyEffect = true;
        StartCoroutine(EffectTimer(_EffectTime));
    }
}
