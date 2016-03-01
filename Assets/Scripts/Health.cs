using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public int BaseHealth = 5;
    public GameObject HealthCanvas;

    [SyncVar]
    public int CurrentHealth;

    private Slider slider;
    private GameObject canvas;
    private RpcNetworkAnimator animator;

    void Awake()
    {
        CurrentHealth = BaseHealth;
        canvas = GameObject.Instantiate(HealthCanvas);
        canvas.transform.SetParent(transform, false);
        slider = GetComponentInChildren<Slider>();
        animator = GetComponent<RpcNetworkAnimator>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (slider != null)
	    {
	        slider.value = (float)CurrentHealth/ (float)BaseHealth;
            canvas.transform.rotation = Quaternion.identity;
	    }
	}

    public void TakeDamage(int value)
    {
        if (GetComponent<AvatarController>() != null && GetComponent<AvatarController>().isHidden)
            return;

        if (GetComponent<AvatarController>() != null)
        {
            if (value > 0 && CurrentHealth > 0) animator.SetTrigger("TakeDamage");
            //Revive. Value < 0 = revive amount
            if (value < 0 && CurrentHealth <= 0) animator.SetBool("Dead", false);
        }

        CurrentHealth -= value;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            if (GetComponent<AvatarController>() != null)
                animator.SetTrigger("Dead");
            else if (GetComponent<MinionController>() != null)
                GetComponent<MinionController>().CmdKill();
        }

        if (CurrentHealth > BaseHealth)
        {
            CurrentHealth = BaseHealth;
        }
    }
}
