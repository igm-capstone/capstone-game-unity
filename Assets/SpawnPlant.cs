using UnityEngine;
using System.Linq;
using System.Collections;

public class SpawnPlant : ISkill
{
    public float distanceFromPlayers = 5f;
    SkillBar mySkillBar;

    public void Awake()
    {
        Name = "SpawnPlant";
        canDrop = false;
        cost = 10;

        mySkillBar = GetComponent<SkillBar>();
    }

    public void Update()
    {
        // If this skill is active, draw UI for all players.
        if (mySkillBar.GetActiveSkill() == this)
        {
            var PlayerList = GameObject.FindGameObjectsWithTag("Player");
            foreach (var plyr in PlayerList)
            {
                plyr.transform.GetChild(3).gameObject.SetActive(true);
            }

        }
        else
        {
            var PlayerList = GameObject.FindGameObjectsWithTag("Player");
            foreach (var plyr in PlayerList)
            {
                plyr.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.layer != LayerMask.NameToLayer("Floor")) return Name + " skill needs to hit the floor. Layer found: " + target.layer.ToString();

        var grid = GridBehavior.Instance;
        var z = grid.transform.position.z;
        clickWorldPos.z = z;

        var node = grid.getNodeAtPos(clickWorldPos);
        if (node == null || !node.canWalk || node.hasLight)
        {
            //Debug.LogFormat("Lit area {0}, unwalkable area {0}", node.hasLight, !node.canWalk);
            return Name + " skill can only be used in the darkness!";
        }

        var avatars = FindObjectsOfType<AvatarController>();
        var sqrDist = distanceFromPlayers * distanceFromPlayers;
        var minDistance = avatars.Select(a => (a.transform.position - clickWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();

        if (avatars.Any() && minDistance < sqrDist)
        {
            //Debug.Log("Near player");
            return Name + " skill cannot be used so close to a player!";
        }

        MinionSpawnManager.Instance.CmdSingleSpawn(clickWorldPos, MinionType.Plant);

        return null;
    }
}
