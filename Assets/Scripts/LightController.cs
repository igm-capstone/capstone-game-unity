using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Light2D))]
public class LightController : MonoBehaviour {
    public enum Status {
        On,
        Off
    }

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
            UpdateLightFX();
            dirty = false;
        }
	}

    public void UpdateLightFX()
    {
        light2d.UpdateLightFX();
    }

    private void DrawSprite() {
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
    }

    public void ToggleStatus()
    {
        switch (CurrentStatus)
        {
            case Status.Off: 
                CurrentStatus = Status.On;
                break;
            case Status.On:
                CurrentStatus = Status.Off;
                break;
        }
        DrawSprite();
    }

}
