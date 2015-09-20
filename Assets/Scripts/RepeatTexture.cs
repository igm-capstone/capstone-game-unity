using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RepeatTexture : MonoBehaviour {
	void Update () {
#if UNITY_EDITOR
        if(!Application.isPlaying)
            gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(transform.localScale.x, transform.localScale.y));
#endif

	}
}