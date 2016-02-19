using UnityEngine;
using System.Collections;

[Rig3DAsset("spawnPoints", Rig3DExports.Position)]
public class AvatarSpawnPoint : MonoBehaviour {
    [Range(1,4)]
    public int PlayerID = 1;
}
