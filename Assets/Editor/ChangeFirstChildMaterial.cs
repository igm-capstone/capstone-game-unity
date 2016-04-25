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

    [MenuItem("CONTEXT/Transform/Change to Normal Wall")]
    private static void NormalWall(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        goTransform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load("Wall") as Material;
    }

    [MenuItem("CONTEXT/Transform/Change to Bathroom Wall")]
    private static void BathroomWall(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        goTransform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load("WallBathroom") as Material;
    }

    [MenuItem("CONTEXT/Transform/Change to Basement Wall")]
    private static void BasementWall(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        goTransform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load("WallBasement") as Material;
    }

    [MenuItem("CONTEXT/Transform/Change to Outside Floor")]
    private static void OutsideFloor(MenuCommand mCommand)
    {
        var goTransform = mCommand.context as Transform;
        var grassGO = GameObject.Find("Grass");

        GameObject outsideFloor = GameObject.Instantiate(Resources.Load("OutsideFloor") as GameObject, Vector3.zero, Quaternion.identity) as GameObject;
        outsideFloor.transform.parent = grassGO.transform;

        outsideFloor.transform.position = goTransform.transform.position;
        outsideFloor.transform.eulerAngles = goTransform.transform.eulerAngles;
        outsideFloor.transform.localScale = goTransform.transform.localScale;

    }

    //[MenuItem("CONTEXT/Transform/Change to Outside Floor Scaled")]
    //private static void OutsideFloorScaled(MenuCommand mCommand)
    //{
    //    var goTransform = mCommand.context as Transform;
    //    var grassGO = GameObject.Find("Grass");

    //    GameObject outsideFloor = GameObject.Instantiate(Resources.Load("OutsideFloorScaled") as GameObject, Vector3.zero, Quaternion.identity) as GameObject;
    //    outsideFloor.transform.parent = grassGO.transform;

    //    outsideFloor.transform.position = goTransform.transform.position;
    //    outsideFloor.transform.eulerAngles = goTransform.transform.eulerAngles;
    //    outsideFloor.transform.localScale = goTransform.transform.localScale;

    //}
}
