using UnityEngine;
using System.Collections;
using System;

public class LanternBehavior : MonoBehaviour
{
    public float LanternDuration;

    public void Initialize(float _duration)
    {
        //Lantern is a pick up lantern
        if (_duration == -1)
        {
            return;
        }

        // Begins counting time for StartUp
        StartCoroutine(DurationTimer(LanternDuration));
    }

    // Duration timer of trap. Destroy itself after some time.
    IEnumerator DurationTimer(float _LanternDuration)
    {
        // Waits for designated time
        yield return new WaitForSeconds(_LanternDuration);

        // Waits for end of frame so all logic related to this can run
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player")
        {
            return;
        }
        if (other.gameObject.GetComponent<SetRegLantern>().CanPickUpLantern())
        {
            other.gameObject.GetComponent<SetRegLantern>().PickUpLantern();
            Destroy(this.gameObject);
        }
    }
}
