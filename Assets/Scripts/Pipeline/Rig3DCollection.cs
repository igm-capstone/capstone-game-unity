using UnityEngine;

public class Rig3DCollection : MonoBehaviour
{
    public string CollectionName;

    [MultiButtons(Columns = 3)]
    public Rig3DExports Exports;

    [Range(-1, 4)]
    public int CullingLayer = -1;
    public bool IsStaticMesh;
    public bool CalculateBounds;
    public bool NormalizeDepth;

    public string TextureName;
}
