using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ObjectiveUI : NetworkBehaviour
{
    public Sprite capturedUI;

    [ClientRpc]
    private void RpcUpdateObjectiveUI(int ID, bool shouldEnableMainHall)
    {
        GameObject domPointCanvas = transform.FindChild(ID.ToString()).gameObject;
        domPointCanvas.GetComponentInChildren<Image>().sprite = capturedUI;
        domPointCanvas.GetComponentInChildren<Image>().color = Color.grey;
        domPointCanvas.GetComponent<Text>().color = Color.grey;

        if (shouldEnableMainHall)
        {
            for (int i = 0; i < 3; i++)
            {
                transform.FindChild(i.ToString()).gameObject.SetActive(false);
            }
            transform.FindChild("3").gameObject.SetActive(true);
            //FindObjectOfType<IndicatorCollector>().ChangeIndicators();
        }
    }

    [Command]
    public void CmdUpdateObjectiveUI(int ID, bool shouldEnableMainHall)
    {
        RpcUpdateObjectiveUI(ID, shouldEnableMainHall);
    }
}
