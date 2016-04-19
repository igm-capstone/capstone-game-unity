using UnityEngine;
using System.Collections;
using UnityEditor;

public class ChangeFirstChildMaterial : MonoBehaviour {

	[MenuItem("CONTEXT/Transform/Change to Bathroom Floor")]
    private static void BathFloor(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        goTransform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load("BathroomFloor") as Material;
    }

    [MenuItem("CONTEXT/Transform/Change to Basement Floor")]
    private static void BasementFloor(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        goTransform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load("BasementFloor") as Material;
    }

    [MenuItem("CONTEXT/Transform/Change to Wood Floor")]
    private static void WoodFloor(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        goTransform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load("WoodFloor") as Material;
    }
}
