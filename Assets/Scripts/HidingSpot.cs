using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[Rig3DAsset("hidingSpots",Rig3DExports.Position | Rig3DExports.Scale | Rig3DExports.Rotation | Rig3DExports.Mesh)]

public class HidingSpot : NetworkBehaviour
{
    [SyncVar]
    public bool isOccupied;

    void Awake()
    {
        isOccupied = false;
    }

    [Export]
    public string meshName;
//    public string meshName
//    {
//        get
//        {
//#if UNITY_EDITOR
//        //    var renderer = transform.Find("Mesh").GetComponent<MeshFilter>();

//            var path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
//            return System.IO.Path.GetFileName(path);
//#else
//            return "";
//#endif
//        }
//    }
    
    public void SetOccupiedStatus(bool _isOccupied)
    {
        isOccupied = _isOccupied;
    }
}
