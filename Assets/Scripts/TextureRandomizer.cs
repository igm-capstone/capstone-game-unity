using UnityEngine;
using System.Collections;

public class TextureRandomizer : MonoBehaviour {

    public Texture[] textures;

	public void RandomizeTexture()
    {
        transform.Find("Model/toad").GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", textures[Random.Range(0,textures.Length)]);
    }
}
