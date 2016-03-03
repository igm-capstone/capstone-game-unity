using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Light2D))]
public class LightController : NetworkBehaviour {
    public enum LghtStatus
    {
        Off,
        On,
        Dimmed
    }

    [SyncVar(hook = "GotStatusFromSrv")]
    public LghtStatus CurrentStatus = LghtStatus.Off;
    //public Sprite SpriteOn;
    //public Sprite SpriteOff;

    Light2D light2d;
    new MeshRenderer renderer;
    new PolygonCollider2D collider;
    private GameObject light3D;
    //SpriteRenderer sprite;

    public bool dirty = true;

    Material onLightMat;
    Material offLightMat;

    // Use this for initialization
    void Start ()
    {

        light2d = GetComponent<Light2D>();
        renderer = GetComponent<MeshRenderer>();
        //sprite = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponentInChildren<PolygonCollider2D>();
        light3D = transform.FindChild("Light3D").gameObject;
        DrawSprite();

        onLightMat = Resources.Load<Material>("LightOnMat");
        offLightMat = Resources.Load<Material>("LightOffMat");

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
                GridBehavior.Instance.SetGridDirty();
                GridBehavior.Instance.SetAIDirty();
            }
        }
	}

    private void DrawSprite() {
            
        switch (CurrentStatus)
        {
            case LghtStatus.Off:
                // Starting state. Lights are off
                //sprite.sprite = SpriteOff;
                //sprite.sprite = SpriteOn;
                light3D.SetActive(false);
                renderer.enabled = false;
                collider.enabled = false;
                break;

            case LghtStatus.On:
                // Lights are On
                //sprite.sprite = SpriteOn;
                light3D.SetActive(true);
                renderer.enabled = true;
                light2d.lightMaterial = onLightMat;
                collider.enabled = true;
                break;

            case LghtStatus.Dimmed:
                // Ghost Dimmed the lights.
                //sprite.sprite = SpriteOff;
                light3D.SetActive(false);
                //light3D.GetComponent<Light>().intensity = 2.0f;
                light2d.lightMaterial = offLightMat;
                collider.enabled = false;
                break;
        }

        var material = GetComponent<MeshRenderer>().sharedMaterial;

        if (material) {
            var c = material.color;
            c.a = 1;
            //sprite.color = c; 
        }
        
    }

    public void ChangeStatusTo(LghtStatus NxtStatus)
    {
        //Debug.Log("ToggleStatus called. IsServer: "+isServer.ToString());
        switch (NxtStatus)
        {
            case LghtStatus.Off: 
                CurrentStatus = LghtStatus.Off;
                DrawSprite();
                break;

            case LghtStatus.On:
                CurrentStatus = LghtStatus.On;
                DrawSprite();
                break;

            case LghtStatus.Dimmed:
                CurrentStatus = LghtStatus.Dimmed;
                DrawSprite();
                break;
        }
        dirty = true;
    }

    [Client]
    void GotStatusFromSrv(LghtStatus latestStatus)
    {
        //Debug.Log("GotSts from server. IsServer: " + isServer.ToString());
        CurrentStatus = latestStatus;
        dirty = true;
    }

}
