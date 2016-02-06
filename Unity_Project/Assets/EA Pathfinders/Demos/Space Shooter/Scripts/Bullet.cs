using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float force;
    public float damage = 10.0f;
	public float lifetime;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.AddRelativeForce(Vector2.up * force);
		Destroy(gameObject, lifetime);
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		Destroy(gameObject);
	}

    void OnCollisionEnter(Collision col)
    {
        Destroy(gameObject);
    }
}
