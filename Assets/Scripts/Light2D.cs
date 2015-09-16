using UnityEngine;
using System.Collections;

public class Light2D : MonoBehaviour {
    private GridBehavior grid;
    Vector3 lastPos;
    public GameObject goal;

	// Use this for initialization
	void Start () {
        grid = FindObjectOfType<GridBehavior>();
        lastPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position != lastPos)
        {
            grid.UpdateCosts();
            lastPos = transform.position;
            grid.GetAStartPath(gameObject, goal);
        }
	}
    void OnMouseDown()
    {
       // grid.GetAStartPath(gameObject, goal);
    }
}
