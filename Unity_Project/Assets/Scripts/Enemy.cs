using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public GameObject target;
	public GameObject bulletPrefab;
	public float thrust;
	public float chaseDistance;
	float force;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		if (target == null) {
			target = GameObject.FindGameObjectWithTag("Player");
		}
		
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		force = 0.0f;
		transform.LookAt(target.transform);
		
		if (Vector2.Distance(transform.position, target.transform.position) > chaseDistance) {
			force = 1.0f;
		}
		
		rb.AddRelativeForce(Vector2.up * thrust * force);
	}
	
	void Fire() {
		Instantiate(bulletPrefab, transform.position, transform.rotation);
	}
}
