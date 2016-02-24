using UnityEngine;
using System.Collections;

public class ExplosionBehavior : MonoBehaviour
{
    float curSize = 0.1f;
    bool mustInflate = true;

    float durTime = 0.5f;
    float startSize = 0.1f;

    // This needs to be 2 times the size of the Explosion radius on the GrenadeToss Script
    float ExploDiameter = 7.0f;

    Vector3 curScale;

    float exploTimer;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(DurationTimer(durTime));

        // Init explosion size
        curSize = startSize;
        curScale.x = curSize;
        curScale.y = curSize;
        curScale.z = curSize;

        // Start timer
        exploTimer = 0.0f;

    }
	
	// Update is called once per frame
	void Update ()
    {

        //Updates timer
        exploTimer += Time.deltaTime / (durTime/2);

        if (mustInflate)
        {
            // Expand
            //curSize = Mathf.Lerp(curSize, radius, Time.deltaTime * durTime);
            curSize = Mathf.Lerp(curSize, ExploDiameter, exploTimer);

        }
        else
        {
            // Contract
            //curSize = Mathf.Lerp(curSize, startSize, Time.deltaTime * durTime);
            curSize = Mathf.Lerp(curSize, startSize, exploTimer);
        }

        // updates Scale
        curScale.x = curSize;
        curScale.y = curSize;
        curScale.z = curSize;


        transform.localScale = curScale;
    }


    // Duration timer of Explosion. Destroy itself after some time.
    IEnumerator DurationTimer(float _ExploTime)
    {
        // Expanding
        yield return new WaitForSeconds(_ExploTime/2);
        // resets timer:
        //exploTimer = 0.0f;
        //mustInflate = false;
        
        // Contracting
        yield return new WaitForSeconds(_ExploTime / 2);
        Destroy(this.gameObject);
    }
}
