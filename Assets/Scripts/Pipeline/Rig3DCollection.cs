using UnityEngine;

public class Rig3DCollection : MonoBehaviour
{
    public string CollectionName;

    [MultiButtons(Columns = 3)]
    public Rig3DExports Exports;

    public bool CalculateBounds;
    public bool NormalizeDepth;
}
