using UnityEngine;
using BladeCast;

public class Player : MonoBehaviour {

    public int playerIndex = 1;
	public float initialHealth = 100;
	public float fireRate = 10.0f;
	public float rotationSpeed = 5.0f;
    public float thrust = 1000.0f;
    public Color shipColor;
    public GameObject bulletPrefab;
	public GameObject explosionPrefab;

    public GameObject[] players;
    float health;
    float fireTimer = 0.0f;
    float force = 0.0f;
    float angle = 0.0f;

    Rigidbody rb;
    BladeCast.BCSender sender;

    void Awake()
    {
        GetComponent<Renderer>().material.SetColor("_Color", shipColor);
    }

	// Use this for initialization
	void Start () {
		BladeCast.BCListener listener = GetComponent<BladeCast.BCListener>();
        sender = GetComponent<BladeCast.BCSender>();
		listener.Add("movement", 0, "Move");
		listener.Add("fire", 0, "Fire");
        listener.Add("spawn", 0, "Spawn");
		rb = GetComponent<Rigidbody>();
        players = GameObject.FindGameObjectsWithTag("Player");
        health = initialHealth;
	}
	
	void OnCollisionEnter(Collision col) {
		float speed = rb.velocity.magnitude;
		health -= speed;

        sender.SendToListeners("health", "health", health, playerIndex);

		if (health <= 0.0f) {
			Instantiate(explosionPrefab, transform.position, transform.rotation);
			Destroy(gameObject);
		}

        if (col.gameObject.CompareTag("Bullet"))
        {
            health -= col.gameObject.GetComponent<Bullet>().damage;
        }
	}
		
	void FixedUpdate() {
		transform.eulerAngles = new Vector3(0,0, angle - 90.0f);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);		
		rb.AddRelativeForce (Vector2.up * force * thrust);
    }

	void Move(ControllerMessage msg) {
		if (msg.ControllerSource == playerIndex) {
			if (msg.Payload.HasField("angle")) {
				string rawAngle = msg.Payload.GetField("angle").ToString ();
				angle = float.Parse (rawAngle);
			}
			
			string rawForce = msg.Payload.GetField ("force").ToString ();
			force = Mathf.Clamp(float.Parse (rawForce), 0, 1.0f);
		}
	}
	
	void Fire(ControllerMessage msg) {
        if (msg.ControllerSource == playerIndex)
        {
            if (Time.time - fireTimer >= 1 / fireRate)
            {
                fireTimer = Time.time;

                Collider collider = GetComponent<Collider>();
                float height = collider.bounds.size.y + 0.25f;

                Vector2 firePosition = transform.position + (transform.up * (height / 2));
                GameObject bullet = (GameObject)Instantiate(bulletPrefab, firePosition, transform.rotation);

                bullet.GetComponent<Rigidbody>().velocity = rb.velocity;
            }
        }
	}

    void Spawn()
    {
        health = initialHealth;
        float x = Random.Range(-25f, 25f);
        float y = Random.Range(-25f, 25f);
        float z = Random.Range(-25f, 25f);
        Vector3 pos = new Vector3(x, y, z);
        transform.position = pos;
    }
}
