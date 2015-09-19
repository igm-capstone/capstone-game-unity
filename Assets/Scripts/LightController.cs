using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Light2D))]
public class LightController : MonoBehaviour {
    public enum Status {
        Empty,
        On,
        Off
    }

    public Status CurrentStatus = Status.On;
    public Sprite SpriteEmpty;
    public Sprite SpriteOn;
    public Sprite SpriteOff;

    Light2D light2d;
    MeshRenderer renderer;
    SpriteRenderer sprite;

	// Use this for initialization
	void Awake () {
        light2d = GetComponent<Light2D>();
        renderer = GetComponent<MeshRenderer>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        DrawSprite();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateLightFX()
    {
        light2d.UpdateLightFX();
    }

    private void DrawSprite() {
        switch (CurrentStatus)
        {
            case Status.Empty:
                sprite.sprite = SpriteEmpty;
                renderer.enabled = false;
                break;
            case Status.On:
                sprite.sprite = SpriteOn;
                renderer.enabled = true;
                break;
            case Status.Off:
                sprite.sprite = SpriteOff;
                renderer.enabled = false;
                break;
        }
    }

    public void ToggleStatus()
    {
        switch (CurrentStatus)
        {
            case Status.Empty:
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
