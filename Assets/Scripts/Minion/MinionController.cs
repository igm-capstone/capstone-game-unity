using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public enum MinionType
{
    Meelee,
    AOEBomber,
    HauntMelee
}

[RequireComponent(typeof(TargetFollower))]
public class MinionController : NetworkBehaviour
{
    private Dictionary<Type, MinionBehaviour> map;
    private Stack<MinionBehaviour> behaviourStack; 

    private float agroTime;
    private GameObject visibleTo;

    public  Transform ClosestAvatar;
    public TargetFollower Follower { get; private set; }

    public GridBehavior grid;

    public MinionType Type;

	// Use this for initialization
	void Awake ()
	{
	    grid = GridBehavior.Instance;
        Follower = GetComponent<TargetFollower>();
        map = new Dictionary<Type, MinionBehaviour>();
        behaviourStack = new Stack<MinionBehaviour>();

	    foreach (var behaviour in GetComponents<MinionBehaviour>())
	    {
	        map.Add(behaviour.GetType(), behaviour);
	    }

	    ActivateBehaviour<WanderAround>();
	}

    public void ActivateBehaviour<T>() where T : MinionBehaviour
    {
        var newBehaviourType = typeof (T);

        if (behaviourStack.Any())
        {
            behaviourStack.Peek().DeactivateBehaviour();
        }

        var nextBehaviour = map[newBehaviourType];
        nextBehaviour.ActivateBehaviour();

        behaviourStack.Push(nextBehaviour);
    }

    public void DeactivateBehaviour()
    {
        behaviourStack.Pop().DeactivateBehaviour();;

        if (behaviourStack.Any())
        {
            behaviourStack.Peek().ActivateBehaviour();
        }
    }

    void Update()
    {
        if (ClosestAvatar == null)
        {
            var pos = transform.position;

            AvatarController[] avatars;
            if (visibleTo)
            {
                avatars = new[] {visibleTo.GetComponent<AvatarController>() };
            }
            else
            {
                avatars = FindObjectsOfType<AvatarController>();
            }

            var orderedAvatars = avatars
                .Where(a => !a.Disabled && !a.isHidden)         // skip disabled and hidden avatars,
                .Select(a => a.transform)                       // select transform
                .OrderBy(t => (t.position - pos).sqrMagnitude);  // and order by distance


            var maxDist = grid.nodeRadius * 2.8f * 15;
            var maxDistSqr = maxDist * maxDist;
            foreach (var avatar in orderedAvatars)
            {
                var dist = avatar.transform.position - transform.position;
                if (dist.sqrMagnitude > maxDistSqr)
                {
                    continue;
                }

                var path = grid.GetFringePath(transform.position, avatar.transform.position, 15);
                if (path.Any())
                {
                    ClosestAvatar = avatar.transform;
                    break;
                }
            }
        }

        if (!behaviourStack.Any())
        {
            ActivateBehaviour<WanderAround>();
        }

        behaviourStack.Peek().UpdateBehaviour();

        Follower.ResetGoal();

        agroTime += Time.deltaTime;

        if (agroTime > 1)
        {
            ClosestAvatar = null;
            agroTime = 0;
        }
    }

    [Command]
    public void CmdKill()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    public void CmdAssignDamage(GameObject obj, int damage)
    {
        if (visibleTo != null && obj != visibleTo)
        {
            return;
        }

        obj.GetComponent<Health>().TakeDamage(damage);
    }

    public void SetVisibility(GameObject obj)
    {
        visibleTo = obj;
    }

    [ClientRpc]
    public void RpcTriggerExplosionSrpite( bool State)
    {
        if (Type == MinionType.AOEBomber)
        {
            GameObject AttackSprite = transform.FindChild("ExplosionSprite").gameObject;
            AttackSprite.SetActive(State);
        }
    }
}
