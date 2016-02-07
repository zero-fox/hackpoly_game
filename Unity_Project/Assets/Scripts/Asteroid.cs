using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	public float maxSpeed;
	public float maxRotation;
	public float minScale;
	public float maxScale;

	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		
		rb.velocity = new Vector2(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed));
		rb.AddRelativeTorque(Random.Range(0.0f, maxRotation), Random.Range(0.0f, maxRotation), Random.Range(0.0f, maxRotation));
		transform.localScale = new Vector3(Random.Range(minScale, maxScale), Random.Range(minScale, maxScale), Random.Range(minScale, maxScale));
		
		rb.mass = rb.mass * transform.localScale.x * transform.localScale.y * transform.localScale.z;
	}
	
	void FixedUpdate() {
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
	}
}
