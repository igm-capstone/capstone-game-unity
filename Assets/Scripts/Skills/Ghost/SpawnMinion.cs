using UnityEngine;
using System.Linq;

public class SpawnMinion : ISkill
{
    SkillBar mySkillBar;

    public float radius = 4f;
    public int spawnCount = 3;
    public override string Name { get { return "Imp"; } }

    Vector3 mouseWorldPos;
    float mouseDistance;
    //AvatarController[] avatars2;

    public void Awake()
    {
        canDrop = false;

        IsSpawnSkill = true;
        MinSpawnDist = 10f;
        MaxSpawnDist = 60f;

        key = KeyCode.Alpha1;

        mySkillBar = GetComponent<SkillBar>();

        // ToolTip text
        ToolTip.Description =       "Spwans 2 Imps.\nAttacks on sight.";
        ToolTip.FirstAttribute=     "Regular";
        ToolTip.SecondAttribute =   "Low";
        ToolTip.ThirdAttribute =    "Fast";

    }

    public void Update()
    {        
        //RaycastHit hit;
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask(new[] { "Floor" })))
        //{
        //    mouseWorldPos = hit.point;
        //    mouseDistance = avatars.Select(a => (a.transform.position - mouseWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();
        //}

        ////change mouse to no spawn indicator
        //if (avatars.Any() && ((mouseDistance < (MinSpawnDist * MinSpawnDist) || (mouseDistance > (MaxSpawnDist * MaxSpawnDist)))))
        //{
        //    Debug.Log("mouse");
        //}

        if (Input.GetKeyDown(key))
        {
            mySkillBar.SetActiveSkill(this);
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

        //var avatars = FindObjectsOfType<AvatarController>();
        //avatars2 = avatars;
        var minSqrDist = MinSpawnDist * MinSpawnDist;
        var maxSqrDist = MaxSpawnDist * MaxSpawnDist;
        var plyrDistance = avatars.Select(a => (a.transform.position - clickWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();


        if (avatars.Any() && ((plyrDistance < minSqrDist) || (plyrDistance > maxSqrDist)))
        {
            //Debug.Log("Near player");
            return Name + " skill cannot be used ouside of spawn circle.";
        }

        MinionSpawnManager.Instance.CmdMultipleSpawn(clickWorldPos, MinionType.Meelee, spawnCount, radius);

        // DebugCode
        foreach (var trgt in avatars)
        {
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.right * MinSpawnDist);
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.up * MaxSpawnDist);
        }

        return null;
    }
}
