using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[Rig3DAsset("dominationPoints", Rig3DExports.Position)]
public class Domination : MonoBehaviour
{
    [System.NonSerialized]
    public bool captured = false;
    private bool outsideDominationArea = true;
    public float elapsedTime = 0.0f;

    [Export("captureTime")]
    public float timeToCapture = 20.0f;
    public Image dominationFill;
    public Image connection;

    // Domination Point Tier. All dom Pnts of a tier must be dominated to go to the next.
    [Export("tier"), Range(0, 5)]
    public int TierCapture;
    public bool canBeCaptured = false;

    public event System.Action WasCaptured;


    public void Update()
    {
        if (outsideDominationArea == true && elapsedTime >= 0.0f)
        {
            elapsedTime -= Time.deltaTime;
            dominationFill.fillAmount = elapsedTime / timeToCapture;
        }

        //if(captured == true && connection != null)
        //{
        //    connection.fillAmount += (Time.deltaTime * 0.5f);
        //}
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && canBeCaptured)
        {
            outsideDominationArea = false;
            if (elapsedTime < timeToCapture)
            {
                elapsedTime += Time.deltaTime;
                dominationFill.fillAmount = elapsedTime / timeToCapture;
            }
            else
            {
                captured = true;
                // Indicates that this point was captured to the EventMngr
                WasCaptured();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player" && captured == false)
        {
            outsideDominationArea = true;
        }
    }
}
