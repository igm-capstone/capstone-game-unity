using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.EventSystems;

public class GhostController : NetworkBehaviour {

    LightController[] lights;
    [SerializeField]
    GameObject ghostUIPrefab;
    [SerializeField]
    LayerMask LayersToClick;

    SkillBar ghostSkillBar;
            
	void Start ()
    {
        lights = FindObjectsOfType<LightController>();

        GameObject ghostUI = Instantiate(ghostUIPrefab);
        ghostUI.transform.SetParent(GameObject.Find("MainCanvas").transform, false);

        ghostSkillBar = ghostUI.GetComponentInChildren<SkillBar>();
    }
	
	// Update is called once per frame
	void Update ()
    {
       HandleInput();
	}

    void HandleInput()
    {
        // Using mouse over instead of ray cast due to 2D collider. Physics does not interact with Physics2D.
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up, 1000, LayersToClick);

            if (hit.collider != null)
            {
                Debug.Log(LayerMask.LayerToName(hit.collider.gameObject.layer));

                var activeSkill = ghostSkillBar.GetActiveSkill();
                if (activeSkill) activeSkill.Use(hit.collider.gameObject);
            }
        }
    }

    [Command]
    public void CmdLightHasBeenClicked(string lightName)
    {
        LightController light = GameObject.Find(lightName).GetComponent<LightController>();
        light.ToggleStatus();
    }

}
