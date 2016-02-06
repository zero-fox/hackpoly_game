using UnityEngine;
using System.Collections;

public class ScrollUV : MonoBehaviour {

	public float speed;
	
	Material material;
	
	void Start() {
		material = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 offset = material.mainTextureOffset;		
		
		offset.x = transform.position.x / transform.localScale.x * material.mainTextureScale.x * speed;
		offset.y = transform.position.y / transform.localScale.y * material.mainTextureScale.y * speed;
		
		material.mainTextureOffset = offset;
	}
}
