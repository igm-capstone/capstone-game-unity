using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SkillBar))]
public class GhostController : MonoBehaviour {
    [SerializeField]
    LayerMask LayersToClick;

    SkillBar _ghostSkillBar;
            
	void Start ()
    {
        _ghostSkillBar = GetComponent<SkillBar>();
	    _ghostSkillBar.enabled = true;
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
            var clickWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickWordPos, -Vector2.up, 1000, LayersToClick);

            if (hit.collider != null)
            {
                var activeSkill = _ghostSkillBar.GetActiveSkill();
                if (activeSkill)
                {
                    activeSkill.Use(hit.collider.gameObject, clickWordPos);
                }
                else
                {
                    HelpMessage.Instance.SetMessage("No skill selected!");
                }
            }
        }
    }
}
