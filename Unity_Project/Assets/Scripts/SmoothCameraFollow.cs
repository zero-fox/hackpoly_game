using UnityEngine;
using System.Collections;

public class SmoothCameraFollow : MonoBehaviour {

	public GameObject target;

	public float cameraSpeed = 1.0f; 

	// Use this for initialization
	void Start () {
		if (target == null) {
			target = GameObject.FindGameObjectWithTag("Player");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			Vector3 newPos = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * cameraSpeed);
			transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);			
		}
	}
}
