using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Domination : MonoBehaviour
{
    [System.NonSerialized]
    public bool captured = false;
    private bool outsideDominationArea = true;
    public float elapsedTime = 0.0f;
    public float timeToCapture = 20.0f;
    public Slider dominationSlider;

    public void Update()
    {
        if (outsideDominationArea == true && elapsedTime >= 0.0f)
        {
            elapsedTime -= Time.deltaTime;
            dominationSlider.value = elapsedTime / timeToCapture;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            outsideDominationArea = false;
            if (elapsedTime < timeToCapture)
            {
                elapsedTime += Time.deltaTime;
                dominationSlider.value = elapsedTime / timeToCapture;
            }
            else
            {
                captured = true;
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
