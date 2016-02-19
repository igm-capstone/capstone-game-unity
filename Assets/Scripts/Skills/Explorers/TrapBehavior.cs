using UnityEngine;
using System.Collections;

public class TrapBehavior : MonoBehaviour {

    CircleCollider2D TriggerCol;
    ParticleSystem PSys = null;

    public TrapType MyType;

    public float TriggerRadius;
    public float Duration;
    public float EffectTime;   // The time in between applications of the trap effect. A "2" would mean that the effect is applied every 2 seconds.
    public bool AffectPlayers = false;

    public int PoisonDamage;
    public float GlueSlowRate;
    public float SpringKnockForce;

    bool mustApplyEffect;
    bool isTrapActive;

    // Use this for initialization
    void Awake ()
    {
        // Component Getters
        TriggerCol = GetComponentInChildren<CircleCollider2D>();
        TriggerCol.radius = TriggerRadius;

        if (MyType == TrapType.Poison)
        {
            PSys = GetComponent<ParticleSystem>();
            PSys.Stop();
        }

        // Starting Values
        mustApplyEffect = false;
        isTrapActive = false;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Minion") && isTrapActive == false)
        {
            ActivateTrap(MyType);
            isTrapActive = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (mustApplyEffect &&(other.gameObject.layer == LayerMask.NameToLayer("Minion") ||
            (AffectPlayers && other.gameObject.layer == LayerMask.NameToLayer("Player"))))
        {
            ApplyTrapEffect(MyType, other.gameObject);
        }
    }

    //Activates the Trap
    void ActivateTrap(TrapType _MyType)
    {
        // Activate trap
        mustApplyEffect = true;

        switch (_MyType)
        {
            case TrapType.Poison:
                // Start Particle system
                PSys.Play();
                PSys.loop = true;

                StartCoroutine(EffectTimer(EffectTime));
                StartCoroutine(DurationTimer(Duration));
                break;

            case TrapType.Glue:
                Debug.Log("Glue Trap not implemented yet");
                break;

            case TrapType.Spring:
                Debug.Log("Spring Trap not implemented yet");
                break;

            default:
                Debug.Log("Activate Trap: TRap type not found");
                break;
        }
    }

    // Applies the Trap Effect on a Target.
    void ApplyTrapEffect(TrapType _MyType, GameObject Target)
    {
        switch (_MyType)
        {
            case TrapType.Poison:
                // Get Player obj. This will return not null for all Explorers.
                GameObject TrapPlayerObj = GameObject.Find("Me");
                
                // Test for a Script exclusive to the traper to avoid aplying damage multiple times.
                if (TrapPlayerObj.GetComponent<SetTrap>() != null)
                {   // Apply Poison Damage
                    TrapPlayerObj.GetComponent<AvatarNetworkBehavior>().CmdAssignDamage(Target, PoisonDamage);
                }
                break;

            case TrapType.Glue:
                Debug.Log("Glue Trap not implemented yet");
                break;

            case TrapType.Spring:
                Debug.Log("Spring Trap not implemented yet");
                break;

            default:
                Debug.Log("Apply Trap Effect: TRap type not found");
                break;
        }

        // Reset for Effect timer.
        mustApplyEffect = false;
    }

    // Wait for Trap Effect Time to apply the Trap effect.
    IEnumerator EffectTimer(float _EffectTime)
    {
        yield return new WaitForSeconds(_EffectTime);
        mustApplyEffect = true;

        // Resets timer for next 
        StartCoroutine(EffectTimer(_EffectTime));
    }

    // Duration timer of trap. Destroy itself after some time.
    IEnumerator DurationTimer(float _TrapDuration)
    {
        yield return new WaitForSeconds(_TrapDuration);
        Destroy(this.gameObject);
    }

    // Draw the Trap circular trigger.
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, TriggerRadius);
    }

}
