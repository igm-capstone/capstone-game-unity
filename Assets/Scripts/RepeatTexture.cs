using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RepeatTexture : MonoBehaviour {

    public float scale = 1;
	void Update () {
#if UNITY_EDITOR
        if(!Application.isPlaying)
            gameObject.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(transform.localScale.x * scale, transform.localScale.y * scale));
#endif

	}
}