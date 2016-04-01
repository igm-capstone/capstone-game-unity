using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SkillBar))]
public class GhostController : MonoBehaviour
{
    [SerializeField]
    LayerMask ClickableLayers;

    SkillBar _ghostSkillBar;

    void Start()
    {
        _ghostSkillBar = GetComponent<SkillBar>();
        _ghostSkillBar.enabled = true;

        DomMngr.Instance.PointDominated += OnControlPointDominated;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void OnControlPointDominated()
    {
        _ghostSkillBar.UpgradeRestoreRate();
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
            if ((hit2d = Physics2D.Raycast(clickWordPos, Vector2.zero, 1000, ClickableLayers)) == true)
            {
                go = hit2d.collider.gameObject;
            }
            else if (Physics.Raycast(clickWordPos + Vector3.back * 500, Vector3.forward, out hit3d, 1000, ClickableLayers))
            {
                go = hit3d.collider.gameObject;
            }
            else
            {
                return;
            }

            if (go.layer == LayerMask.NameToLayer("LightSwitch"))
            {
                // Hit a Light. Disable it and move on.
                DisableLight(go);
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

            if (activeSkill.Use(go, clickWordPos))
            {
                _ghostSkillBar.EnergyLeft -= activeSkill.cost;
            }
        } //if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())

        /*
        // Right-Click deselects skills.
        if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            _ghostSkillBar.SetActiveSkill(GetComponent<DisableLight>());
        }
        */
        
    }

    void DisableLight(GameObject target)
    {
        var light = target.GetComponent<LightController>();

        if (light != null && light.CurrentStatus == LightController.LightStatus.On)
        {
            light.ChangeStatusTo(LightController.LightStatus.Dimmed); //Disabled
            StartCoroutine(flickerToOff(light));
        }
        return;
    }


    // Flicker Light Subrotine.
    IEnumerator flickerToOff(LightController light)
    {
        yield return new WaitForSeconds(0.05f);
        light.ChangeStatusTo(LightController.LightStatus.On); //On
        yield return new WaitForSeconds(0.1f);
        light.ChangeStatusTo(LightController.LightStatus.Dimmed); //Disabled
        yield return new WaitForSeconds(0.05f);
        light.ChangeStatusTo(LightController.LightStatus.On); //On
        yield return new WaitForSeconds(0.1f);
        light.ChangeStatusTo(LightController.LightStatus.Dimmed); //Disabled
    }
}
