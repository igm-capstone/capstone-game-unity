using UnityEngine;
using System.Collections;

[Rig3DAsset("spawnPoints", Rig3DExports.Position)]
public class AvatarSpawnPoint : MonoBehaviour {
    [Export("playerId"), Range(0,4)]
    public int PlayerID = 1;
}
