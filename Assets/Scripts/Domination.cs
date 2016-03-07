using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

[Rig3DAsset("dominationPoints", Rig3DExports.Position)]
public class Domination : NetworkBehaviour
{
    [System.NonSerialized]
    public bool captured = false;
    private bool outsideDominationArea = true;

    [SyncVar]
    public float elapsedTime = 0.0f;

    [Export("captureTime")]
    public float timeToCapture = 20.0f;
    public Image dominationFill;
    public Image connection;    
    public float fillAmount;

    // Domination Point Tier. All dom Pnts of a tier must be dominated to go to the next.
    [Export("tier"), Range(0, 5)]
    public int TierCapture;

    [SyncVar]
    public bool canBeCaptured = false;

    public event System.Action WasCaptured;

    public void Update()
    {

        if (isServer && outsideDominationArea == true && elapsedTime >= 0.0f)
        {
            elapsedTime -= Time.deltaTime;
        }

        dominationFill.fillAmount = elapsedTime / timeToCapture;
        fillAmount = dominationFill.fillAmount;
        
        //if(captured == true && connection != null)
        //{
        //    connection.fillAmount += (Time.deltaTime * 0.5f);
        //}
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (isServer && !captured  && canBeCaptured && other.tag == "Player")
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
        if(isServer && !captured && other.tag == "Player")
        {

            outsideDominationArea = true;
        }
    }
}
