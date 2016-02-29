using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExplorerIndicator : MonoBehaviour
{
    public GameObject explorer;
    public GameObject explorerIndicator;
    Vector2 screenCenter;
    Vector2 screenSize;

    void Start()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        //offset (0,0) to center of screen
        screenCenter -= (screenSize / 2);
    }
    /*
    //void Update()
    //{
    //    Vector3 explorerPos = Camera.main.WorldToScreenPoint(explorer.transform.position);
    //    Debug.Log(explorerPos);
    //    if(explorerPos.x > 0 && explorerPos.x < Screen.width &&
    //       explorerPos.y > 0 && explorerPos.y < Screen.height)
    //    {
    //        Debug.Log("Explorer is on screen");
    //    }
    //    else
    //    {
    //        Debug.Log("Explorer is NOT on screen");

    //        float m = explorerPos.y / explorerPos.x;
    //        Vector2 arrowPos = Vector2.zero;
    //        //top
    //        if (explorerPos.y > 0)
    //        {
    //            //right
    //            if(explorerPos.x > 0)
    //            {
    //                //y = mx
    //                float x = screenCenter.x + (Screen.width / 2);
    //                float y = m * x;
    //                arrowPos = new Vector2(x, y);
    //            }
    //            //left
    //            else
    //            {
    //                float x = screenCenter.x - (Screen.width / 2);
    //                float y = m * x;
    //                arrowPos = new Vector2(x, y);
    //            }
    //        }
    //        else
    //        {
    //            //right
    //            if (explorerPos.x > 0)
    //            {
    //                float x = (screenCenter.y + Screen.height / 2)/m;
    //            }
    //            //left
    //            else
    //            {
    //                float x = (screenCenter.y - Screen.height / 2) / m;
    //            }
    //        }
    //        Debug.Log(arrowPos);
    //    }
    //}
    void Update()
    {
        //foreach (GameObject exp in explorers)
        //{
        Vector3 expScreenPos = Camera.main.WorldToScreenPoint(explorer.transform.position);
        if (//expScreenPos.z > 0 &&
            expScreenPos.x > 0 && expScreenPos.x < Screen.width &&
            expScreenPos.y > 0 && expScreenPos.y < Screen.height)
        {
            Debug.Log("Other Explorer is on screen");
            Debug.Log("Screen size " + Screen.width + Screen.height);
            Debug.Log("Expl pos " + expScreenPos);
        }
            else
            {
            //if (screenPos.z < 0)
            //    screenPos *= -1;

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            //make 00 the center of the screen
            expScreenPos -= screenCenter;
            Debug.Log(expScreenPos);
            //find angle from center to explorer
            float angle = Mathf.Atan2(expScreenPos.y, expScreenPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
             
            expScreenPos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

            //float m = cos / sin;

            //Vector3 screenBounds = screenCenter * 0.9f;

            ////Up and down
            //if (cos > 0)
            //    screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            //else
            //    screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

            ////if out of bounds, get the correct side
            //if (screenPos.x > screenBounds.x)
            //    screenPos = new Vector3(screenBounds.x, screenBounds.x / m, 0);
            //else if (screenPos.x < screenBounds.x)
            //    screenPos = new Vector3(-screenBounds.x, -screenBounds.x / m, 0);

            //screenPos += screenCenter;

            //explorerIndicator.transform.localPosition = screenPos;
            //explorerIndicator.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }

        //}
    }
    */
}