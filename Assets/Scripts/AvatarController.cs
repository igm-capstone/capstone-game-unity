﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AvatarController : NetworkBehaviour 
{
    private Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private LayerMask moveableObjectLayer;
    [SerializeField]
    private float collisionRadius;
    private Collider2D[] moveableObstacle = new Collider2D[1];
    private bool disable = false;

    MeleeWeaponBehavior weaponBehavior;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        weaponBehavior = GetComponentInChildren<MeleeWeaponBehavior>();
	}
	
	void FixedUpdate () 
    {
        if(!disable)
            move();
        moveableObstacle = Physics2D.OverlapCircleAll(transform.position, collisionRadius, moveableObjectLayer);
        if (moveableObstacle.Length > 0 && moveableObstacle[0].gameObject != null)
            moveObject();

        //Test Inputs
        // Restart the Level
        if(Input.GetKey(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        // Close Application
        if ((Input.GetKey(KeyCode.F4))&&(Input.GetKey(KeyCode.RightAlt)|| Input.GetKey(KeyCode.LeftAlt)))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.M))
        {
            weaponBehavior.StartCoroutine(weaponBehavior.Slash());
        }
    }

    private void move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg, Vector3.forward);
        rb.velocity = new Vector2(horizontal,vertical) * moveSpeed;
    }

    private void moveObject()
    {
        if(Input.GetKey(KeyCode.Space))
            moveableObstacle[0].transform.parent = gameObject.transform;
        else
            moveableObstacle[0].transform.parent = null;
    }

    private void MeleeAttack()
    {

    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, collisionRadius);
    }
    #endif

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            CmdEndGame("Prisoner Wins!");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && 
             ( collision.contacts[0].collider.gameObject.layer == LayerMask.NameToLayer("Player") ||
                collision.contacts[0].otherCollider.gameObject.layer == LayerMask.NameToLayer("Player") ) )
        {
            disable = true;
            //CmdEndGame("Guard Wins!");
        }
        else if((collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
                 (collision.contacts[0].collider.gameObject.layer == LayerMask.NameToLayer("Player") ||
                    collision.contacts[0].otherCollider.gameObject.layer == LayerMask.NameToLayer("Player"))))
        {
            disable = false;
        }
    }

    [Command]
    void CmdEndGame(string msg)
    {
        RpcEndGame(msg);
    }

    [ClientRpc]
    void RpcEndGame(string msg){
        GameStateHUD hud = FindObjectOfType<GameStateHUD>();
        hud.SetMsg(msg);
    }
}
