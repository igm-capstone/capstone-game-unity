using UnityEngine;
using System.Collections;

public class TrapBehavior : MonoBehaviour {

    CircleCollider2D TriggerCol;
    ParticleSystem PSys = null;


    // Get Player obj. This will return not null for all Explorers.
    GameObject TrapPlayerObj;


    public TrapType MyType;

    public float trapTriggerRadius;
    public float Duration;
    public float EffectTime;   // The time in between applications of the trap effect. A "2" would mean that the effect is applied every 2 seconds.
    public bool AffectPlayers = false;

    public int PoisonDamage;
    public float GlueSlowRate = 0.3f;
    public float SpringKnockForce;

    bool mustApplyEffect;
    bool isTrapActive;

    LayerMask hitLayers;

    // Use this for initialization
    void Awake ()
    {
        // Component Getters
        TriggerCol = GetComponentInChildren<CircleCollider2D>();
        TriggerCol.radius = trapTriggerRadius;

        TrapPlayerObj = GameObject.Find("Me");

        // Starting Values
        mustApplyEffect = false;
        isTrapActive = false;

        if (MyType == TrapType.Poison || MyType == TrapType.Glue)
        {
            PSys = GetComponent<ParticleSystem>();
            PSys.Stop();
        }

        if (MyType == TrapType.Glue)
        {
            mustApplyEffect = true;
        }

        hitLayers = 1 << LayerMask.NameToLayer("Minion") | 1 << LayerMask.NameToLayer("Player");

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTrapActive == false && (other.gameObject.layer == LayerMask.NameToLayer("Minion") ||
                                        (AffectPlayers && other.gameObject.layer == LayerMask.NameToLayer("Player"))))
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

    void OnTriggerExit2D(Collider2D other)
    {
        if (MyType == TrapType.Glue) 
        {
            if (TrapPlayerObj.GetComponent<SetTrapGlue>() != null)
            {
                // Stop glue trap effect on target
                TrapPlayerObj.GetComponent<AvatarNetworkBehavior>().CmdStopSlowDown(other.gameObject);
            }
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
                // Start Particle system
                PSys.Play();
                PSys.loop = true;
                
                //mustApplyEffect is always true for GlueTrap
                StartCoroutine(DurationTimer(Duration));
                break;

            case TrapType.Spring:
                Debug.Log("Spring Trap not implemented yet");
                break;

            default:
                Debug.Log("Activate Trap: Trap type not found");
                break;
        }
    }

    // Applies the Trap Effect on a Target.
    void ApplyTrapEffect(TrapType _MyType, GameObject TargetObj)
    {
        

        switch (_MyType)
        {
            case TrapType.Poison:
                // Test for a Script exclusive to the traper to avoid aplying damage multiple times.
                if (TrapPlayerObj.GetComponent<SetTrapPoison>() != null)
                {   // Apply Poison Damage
                    TrapPlayerObj.GetComponent<AvatarNetworkBehavior>().CmdAssignDamage(TargetObj, PoisonDamage);
                }
                break;

            case TrapType.Glue:
                // Test for a Script exclusive to the traper to avoid setting target to slow multiple times.
                if (TrapPlayerObj.GetComponent<SetTrapGlue>() != null)
                {   // Apply Glue slow down
                    TrapPlayerObj.GetComponent<AvatarNetworkBehavior>().CmdStartSlowDown(TargetObj, GlueSlowRate);
                }
                break;

            case TrapType.Spring:
                Debug.Log("Spring Trap not implemented yet");
                break;

            default:
                Debug.Log("Apply Trap Effect: Trap type not found");
                break;
        }

        if (MyType != TrapType.Glue)
        {
            // Reset for Effect timer.
            mustApplyEffect = false;
        }
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

        if (MyType == TrapType.Glue)
        {

            // Stop slowdown on all current affected targets before destroying trap.
            var curTrgts = Physics2D.OverlapCircleAll(transform.position, trapTriggerRadius, hitLayers);

            foreach (var trgt in curTrgts)
            {
                if (TrapPlayerObj.GetComponent<SetTrapGlue>() != null)
                {
                    
                    if (trgt.GetComponent<AvatarController>() != null ||
                        trgt.GetComponent<TargetFollower>() != null)
                    {
                    
                        // Stop glue trap effect on target
                        TrapPlayerObj.GetComponent<AvatarNetworkBehavior>().CmdStopSlowDown(trgt.gameObject);
                    }// if
                }// if
            }// foreach
        }// if

        // Waits for end of frame so all logic related to this can run
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }

    // Draw the Trap circular trigger.
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, trapTriggerRadius);
    }

}
