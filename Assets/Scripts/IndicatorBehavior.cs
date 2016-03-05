using UnityEngine;
using System.Collections;

public class IndicatorBehavior : MonoBehaviour {

    public GameObject avatar;
    public GameObject domPoint;
    public float padding;
    public Sprite Sprinter;
    public Sprite Support;
    public Sprite Trapper;

    // Update is called once per frame
    void Update ()
    {
        if(domPoint != null)
        {
            DomPointIndicator();
        }

        if (!avatar)
        {
            return;
        }

        var avatarClient = GameObject.Find("Me");

     //   Vector3 meToAvatar = Vector3.Normalize(avatar.transform.position - avatarClient.transform.position);

        Vector2 meScreenPoint = Camera.main.WorldToViewportPoint(avatarClient.transform.position);
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(avatar.transform.position);
        

        float flipZ = (viewportPoint.z < 0.0f) ? -1.0f : 1.0f;
       

        RectTransform canvasRect = GetComponent<RectTransform>();

        Vector2 screenPoint = new Vector2();
        screenPoint.x = ((viewportPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)) * flipZ;
        screenPoint.y = ((viewportPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) * flipZ;

        

        //Debug.Log("VP: " + viewportPoint + " \tSP: " + screenPoint);

        GameObject uiimage = transform.FindChild("UIImage").gameObject;        
        RectTransform imageRect = uiimage.GetComponent<RectTransform>();

        Vector2 halfCanvasSizeDelta = canvasRect.sizeDelta * 0.5f;

        Vector2 clampedScreenPoint = new Vector2();
        clampedScreenPoint.x = Mathf.Clamp(screenPoint.x, -halfCanvasSizeDelta.x + padding, halfCanvasSizeDelta.x - padding);
        clampedScreenPoint.y = Mathf.Clamp(screenPoint.y, -halfCanvasSizeDelta.y + padding, halfCanvasSizeDelta.y - padding);
   
        if (-halfCanvasSizeDelta.x < screenPoint.x && screenPoint.x < halfCanvasSizeDelta.x &&
            -halfCanvasSizeDelta.y < screenPoint.y && screenPoint.y < halfCanvasSizeDelta.y)
        {
            uiimage.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        }
        else
        {
            uiimage.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }

        uiimage.GetComponent<RectTransform>().anchoredPosition = clampedScreenPoint;

       
    }

    void DomPointIndicator()
    {
        Vector3 viewportDomPoint = Camera.main.WorldToViewportPoint(domPoint.transform.position);
        float flipZdomPoint = (viewportDomPoint.z < 0.0f) ? -1.0f : 1.0f;

        RectTransform canvasRect = GetComponent<RectTransform>();

        Vector2 screenPointDomPoint = new Vector2();
        screenPointDomPoint.x = ((viewportDomPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)) * flipZdomPoint;
        screenPointDomPoint.y = ((viewportDomPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) * flipZdomPoint;

        GameObject uiimageDomPoint = transform.FindChild("UIImageGrey").gameObject;
        Vector2 halfCanvasSizeDelta = canvasRect.sizeDelta * 0.5f;

        Vector2 clampedScreenPointDomPoint = new Vector2();
        clampedScreenPointDomPoint.x = Mathf.Clamp(screenPointDomPoint.x, -halfCanvasSizeDelta.x + padding, halfCanvasSizeDelta.x - padding);
        clampedScreenPointDomPoint.y = Mathf.Clamp(screenPointDomPoint.y, -halfCanvasSizeDelta.y + padding, halfCanvasSizeDelta.y - padding);


        if (-halfCanvasSizeDelta.x < screenPointDomPoint.x && screenPointDomPoint.x < halfCanvasSizeDelta.x &&
           -halfCanvasSizeDelta.y < screenPointDomPoint.y && screenPointDomPoint.y < halfCanvasSizeDelta.y)
        {
            uiimageDomPoint.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        }
        else
        {
            uiimageDomPoint.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }

        uiimageDomPoint.GetComponent<RectTransform>().anchoredPosition = clampedScreenPointDomPoint;

    }
}
