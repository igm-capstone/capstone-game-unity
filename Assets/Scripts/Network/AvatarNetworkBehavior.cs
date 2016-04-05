using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//[RequireComponent(typeof(MeleeWeaponBehavior))]
public class AvatarNetworkBehavior : BasePlayerNetworkBehavior
{
    //private MeleeWeaponBehavior meleeBehaviour;

    public override void OnStartLocalPlayer()
    {
        GetComponentInChildren<AvatarController>().enabled = true;
        GetComponentInChildren<MovementBroadcast>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
        GetComponentInChildren<AudioSource>().enabled = true;
        //GetComponentInChildren<EquippedSkill>().enabled = true;
        base.OnStartLocalPlayer();
    }

    void Start()
    {
        foreach (IndicatorCollector i in GameObject.FindObjectsOfType<IndicatorCollector>())
        {
            i.RefreshIndicators();
            i.DomPointIndicators();
            
        }
    }

    void OnDestroy()
    {
        foreach (IndicatorCollector i in GameObject.FindObjectsOfType<IndicatorCollector>())
        {
            i.RefreshIndicators();
        }
    }

    

    [Command]
    public void CmdTakeBlockOver(string block, bool status)
    {
        RpcTakeBlockOver(block, status);
    }

    [ClientRpc]
    void RpcTakeBlockOver(string block, bool status)
    {
        GetComponentInChildren<MoveBlock>().TakeBlockOver(block, status);
    }

    [ClientRpc]
    public void RpcDisableMinion(GameObject minion)
    {
        if (hasAuthority)
        {
            foreach (var s in minion.GetComponentsInChildren<Renderer>())
            {
                s.enabled = false;
            }
            foreach (var s in minion.GetComponentsInChildren<Collider2D>())
            {
                s.enabled = false;
            }
            foreach (var s in minion.GetComponentsInChildren<Canvas>())
            {
                s.gameObject.SetActive(false);
            }
        }
    }

    [Command]
    public void CmdAssignDamage(GameObject obj, int damage)
    {
        // Assign Damage
        obj.GetComponent<Health>().TakeDamage(damage);
    }


    #region Slow Down functions
    [Command]
    public void CmdStartSlowDown(GameObject _obj, float _slowRate)
    {
        RpcStartSlowDown(_obj, _slowRate);
    }

    [ClientRpc]
    public void RpcStartSlowDown(GameObject _obj, float _slowRate)
    {
        if (_obj && _obj.GetComponent<AvatarController>() != null)
            _obj.GetComponent<AvatarController>().StartSlowDown(_slowRate);

        else if (_obj && _obj.GetComponent<TargetFollower>() != null)
            _obj.GetComponent<TargetFollower>().StartSlowDown(_slowRate);
    }

    [Command]
    public void CmdStopSlowDown(GameObject _obj)
    {
        RpcStopSlowDown(_obj);
    }

    [ClientRpc]
    public void RpcStopSlowDown(GameObject _obj)
    {
        if (_obj.GetComponent<AvatarController>() != null)
            _obj.GetComponent<AvatarController>().StopSlowDown();

        else if (_obj.GetComponent<TargetFollower>() != null)
            _obj.GetComponent<TargetFollower>().StopSlowDown();
    }

    #endregion

    [Command]
    public void CmdAssignDamageWithForce(GameObject obj, int damage, float KnockBackAmount)
    {
        if(!obj)
        {
            return;
        }

        // Assign Damage
        obj.GetComponent<Health>().TakeDamage(damage);

        // Calculate KnockBackForce
        KnockBackAmount += .5f;
        Vector3 direction = (obj.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, direction, KnockBackAmount, GridBehavior.Instance.opaqueObstacleMask);
        float distance = (hit ? hit.distance : KnockBackAmount) - .5f;

        
        // Apply Knock back and start stun timer.
        obj.transform.Translate(direction * distance, Space.Self);

        // Stun on minion.
        if (obj.GetComponent<TargetFollower>() != null)
        {
            obj.GetComponent<TargetFollower>().StartStunTimer();
        }
    }

    [Command]
    public void CmdPickup(GameObject obj)
    {
        RpcPickup(obj);
        NetworkServer.Destroy(obj);
    }

    [Command]
    public void CmdDoor(GameObject obj)
    {
        if (!obj.GetComponent<Door>().isSwinging)
        {
            RpcDoor(obj);
        }
    }

    [ClientRpc]
    public void RpcDoor(GameObject obj)
    {
        obj.GetComponent<Door>().SwingDoor();
    }

    [ClientRpc]
    public void RpcPickup(GameObject obj)
    {
        Destroy(obj);
    }
    
    public GameObject ExploPreFab;
    [Command]
    public void CmdSpawnExplosion(Vector3 _exploSpawnPos, float _radius)
    {
        // Instantiate Explosion
        GameObject exploInstance = Instantiate(ExploPreFab, _exploSpawnPos, Quaternion.identity) as GameObject;
        exploInstance.GetComponent<ExplosionBehavior>().ExploRadius = _radius;
        NetworkServer.Spawn(exploInstance);
    }

    [Command]
    public void CmdSetTrapExplorer(Vector3 _position, TrapType _slctdTrap)
    {
        TrapSpawnManager.Instance.CmdSetTrap(_position, _slctdTrap);
    }

    [Command]
    public void CmdSetLantExplorer(Vector3 _position, LanternType _slctdLant, float _duration)
    {
        LanternSpawnManager.Instance.CmdSetLantern(_position, _slctdLant, _duration);
    }

    [Command]
    public void CmdChangeLightStatus(GameObject LightCtrl, LightController.LightStatus NxtStatus)
    {
        LightCtrl.GetComponent<LightController>().ChangeStatusTo(NxtStatus);
    }

    [Command]
    public void CmdHideExplorer(GameObject explorer, GameObject hideObject)
    {
        RpcHideExplorer(explorer);
        hideObject.GetComponentInParent<HidingSpot>().SetOccupiedStatus(true);
    }

    [ClientRpc]
    private void RpcHideExplorer(GameObject explorer)
    {
        explorer.transform.FindChild("AvatarRotation").gameObject.SetActive(false);
        explorer.transform.FindChild("ClassAura").gameObject.SetActive(false);
        explorer.transform.FindChild("HealthCanvas(Clone)").gameObject.SetActive(false);
        explorer.GetComponent<AvatarController>().isHidden = true;        
    }

    [Command]
    public void CmdShowExplorer(GameObject explorer, GameObject hideObject)
    {
        RpcShowExplorer(explorer);
        hideObject.GetComponentInParent<HidingSpot>().SetOccupiedStatus(false);
    }

    [ClientRpc]
    private void RpcShowExplorer(GameObject explorer)
    {
        explorer.transform.FindChild("AvatarRotation").gameObject.SetActive(true);
        explorer.transform.FindChild("ClassAura").gameObject.SetActive(true);
        explorer.transform.FindChild("HealthCanvas(Clone)").gameObject.SetActive(true);
        explorer.GetComponent<AvatarController>().isHidden = false;
    }
    

}
