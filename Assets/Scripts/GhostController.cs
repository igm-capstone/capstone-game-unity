using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SkillBar))]
public class GhostController : MonoBehaviour
{
    [SerializeField]
    LayerMask LayersToClick;

    SkillBar _ghostSkillBar;

    void Start()
    {
        _ghostSkillBar = GetComponent<SkillBar>();
        _ghostSkillBar.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Using mouse over instead of ray cast due to 2D collider. Physics does not interact with Physics2D.
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var clickWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Physics2D.Raycast(clickWordPos, Vector2.zero, 1000, LayerMask.GetMask(new[] { "Door" })))
            {
                return;
            }

            RaycastHit hit3d;
            RaycastHit2D hit2d;
            GameObject go = null;
            if ((hit2d = Physics2D.Raycast(clickWordPos, Vector2.zero, 1000, LayersToClick)) == true)
            {
                go = hit2d.collider.gameObject;
            }
            else if (Physics.Raycast(clickWordPos + Vector3.back * 500, Vector3.forward, out hit3d, 1000, LayersToClick))
            {
                go = hit3d.collider.gameObject;
            }
            else
            {
                return;
            }

            var activeSkill = _ghostSkillBar.GetActiveSkill();
            if (!activeSkill)
            {
                HelpMessage.Instance.SetMessage("No skill selected!");
                return;
            }

            if (_ghostSkillBar.EnergyLeft < activeSkill.cost)
            {
                HelpMessage.Instance.SetMessage("Not enough energy!");
                return;
            }

            if (activeSkill.IsReady())
            {
                _ghostSkillBar.EnergyLeft -= activeSkill.cost;
            }

            activeSkill.Use(go, clickWordPos);
        } //if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())

        /*
        // Right-Click deselects skills.
        if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            _ghostSkillBar.SetActiveSkill(GetComponent<DisableLight>());
        }
        */
        
    }
}
