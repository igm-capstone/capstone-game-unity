using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Light2D))]
public class LightController : NetworkBehaviour {
    public enum Status {
        On,
        Off
    }

    [SyncVar(hook = "GotStatusFromSrv")]
    public Status CurrentStatus = Status.On;
    public Sprite SpriteOn;
    public Sprite SpriteOff;

    Light2D light2d;
    MeshRenderer renderer;
    PolygonCollider2D collider;
    SpriteRenderer sprite;

    public bool dirty = true;

	// Use this for initialization
	void Awake () {
        light2d = GetComponent<Light2D>();
        renderer = GetComponent<MeshRenderer>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponentInChildren<PolygonCollider2D>();
        DrawSprite();
	}

    public LayerMask shadowMask { get { return light2d.shadowMask;} }

	// Update is called once per frame
	void Update () {
	    if (dirty) {
            //Debug.Log("Dirty light");
            DrawSprite();
            light2d.UpdateLightFX();
            dirty = false;

            if (isServer)
            {
                GridBehavior grid = FindObjectOfType<GridBehavior>();
                grid.SetGridDirty();
                grid.SetAIDirty();
            }
        }
	}

    private void DrawSprite() {
        //Debug.Log("DrawSprite");
            
        switch (CurrentStatus)
        {
            case Status.On:
                sprite.sprite = SpriteOn;
                renderer.enabled = true;
                collider.enabled = true;
                break;
            case Status.Off:
                sprite.sprite = SpriteOff;
                renderer.enabled = false;
                collider.enabled = false;
                break;
        }
        var material = GetComponent<MeshRenderer>().sharedMaterial;

        if (material) {
            var c = material.color;
            c.a = 1;
            sprite.color = c; 
        }
        
    }

    public void ToggleStatus()
    {
        //Debug.Log("ToggleStatus called. IsServer: "+isServer.ToString());
        switch (CurrentStatus)
        {
            case Status.Off: 
                CurrentStatus = Status.On;
                break;
            case Status.On:
                CurrentStatus = Status.Off;
                break;
        }
        dirty = true;
    }

    [Client]
    void GotStatusFromSrv(Status latestStatus)
    {
        //Debug.Log("GotSts from server. IsServer: " + isServer.ToString());
        CurrentStatus = latestStatus;
        dirty = true;
    }

}
