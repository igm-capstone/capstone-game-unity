using UnityEngine;
using System.Collections;

public class characterButton : MonoBehaviour {

	public GameObject frog;
	
	
	
	private Rect FpsRect ;
	private string frpString;
	
	private GameObject instanceObj;
	public GameObject[] gameObjArray=new GameObject[10];
	public AnimationClip[] AniList  = new AnimationClip[4];
	
	float minimum = 2.0f;
	float maximum = 50.0f;
	float touchNum = 0f;
	string touchDirection ="forward"; 
	private GameObject toad;
	
	// Use this for initialization
	void Start () {
		
		//frog.animation["dragon_03_ani01"].blendMode=AnimationBlendMode.Blend;
		//frog.animation["dragon_03_ani02"].blendMode=AnimationBlendMode.Blend;
		//Debug.Log(frog.GetComponent("dragon_03_ani01"));
		
		//Instantiate(gameObjArray[0], gameObjArray[0].transform.position, gameObjArray[0].transform.rotation);
	}
	
 void OnGUI() {
	  if (GUI.Button(new Rect(20, 20, 70, 40), "Idle")){
		 frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Idle");
	  }
		  if (GUI.Button(new Rect(90, 20, 70, 40), "Walk")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Walk");
	  }
		 
		  if (GUI.Button(new Rect(160, 20, 70, 40), "Run")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Run");
	  }
		if (GUI.Button(new Rect(230, 20, 70, 40), "Jump")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Jump");
	  }
	    if (GUI.Button(new Rect(300, 20, 70, 40), "Attack")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Attack");
	  }
		  if (GUI.Button(new Rect(370, 20, 70, 40), "Attack01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Attack01");
	 
	  }
			  if (GUI.Button(new Rect(440, 20, 70, 40), "Damage")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Damage");
	  }
			  if (GUI.Button(new Rect(510, 20, 70, 40), "Down")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Down");
	  }
			  if (GUI.Button(new Rect(580, 20, 70, 40), "Up")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Up");
	  }
			  if (GUI.Button(new Rect(650, 20, 70, 40), "Stun")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Stun");
	  }
			  if (GUI.Button(new Rect(720, 20, 70, 40), "Dead")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Dead");
	  }
					  if (GUI.Button(new Rect(790, 20, 70, 40), "Dead01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Dead01");
	  }
				  if (GUI.Button(new Rect(20, 540, 100, 40), "Ver 2.0")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("Mon_T_Idle");
	  }
		
		////////////////////////////////////////////////////////////////////
		if (GUI.Button(new Rect(20, 280, 100, 40),"Toons")){
	       Application.LoadLevel(0);
	 }
		if (GUI.Button(new Rect(20, 320, 100, 40),"Origin")){
	       Application.LoadLevel(1);
	 }
		if (GUI.Button(new Rect(20, 360, 100, 40),"Real")){
	       Application.LoadLevel(2);
	 }
		
		
	   
 }
	
	// Update is called once per frame
	void Update () {
		
		//if(Input.GetMouseButtonDown(0)){
		
			//touchNum++;
			//touchDirection="forward";
		 // transform.position = new Vector3(0, 0,Mathf.Lerp(minimum, maximum, Time.time));
			//Debug.Log("touchNum=="+touchNum);
		//}
		/*
		if(touchDirection=="forward"){
			if(Input.touchCount>){
				touchDirection="back";
			}
		}
	*/
		 
		//transform.position = Vector3(Mathf.Lerp(minimum, maximum, Time.time), 0, 0);
	if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
		//frog.transform.Rotate(Vector3.up * Time.deltaTime*30);
	}
	
}
