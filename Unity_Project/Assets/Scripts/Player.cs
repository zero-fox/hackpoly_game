using UnityEngine;
using BladeCast;

public class Player : MonoBehaviour {

    static public int playerIndex = 1;
	public string playerId;
	public int initialHealth = 100;
	public float fireRate = 10.0f;
	public float rotationSpeed = 5.0f;
    public float thrust = 1000.0f;
    public Color shipColor;
    public GameObject bulletPrefab;
	public GameObject explosionPrefab;
	public int myPoints = 0;

	float attackRange = 10.0f;

    public GameObject[] players;
    float health;
    float fireTimer = 0.0f;
    float force = 0.0f;
    float angle = 0.0f;

    Rigidbody rb;
    BladeCast.BCSender sender;

	GameObject textObj;

    void Awake()
    {
        GetComponent<Renderer>().material.SetColor("_Color", shipColor);
		textObj = transform.GetChild (0).GetChild (0).gameObject;
    }

	// Use this for initialization
	void Start () {
		//how to do stuffff
		BladeCast.BCListener listener = GetComponent<BladeCast.BCListener>();
        sender = GetComponent<BladeCast.BCSender>();

		listener.Add("movement", 0, "Move");
		listener.Add("jump", 0, "Jump");
        listener.Add("attack", 0, "Attack");
		rb = GetComponent<Rigidbody>();
        
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			rb.AddForce (Vector3.right * -100.0f);
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			rb.AddForce (Vector3.up * 100.0f);
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			rb.AddForce (Vector3.right * 100.0f);
		}
	}

	void FixedUpdate() {
		textObj.transform.position = GameManager.Instance.mainCamera.WorldToScreenPoint (transform.position);	
	}

	void Move(ControllerMessage msg) {

		if (msg.ControllerSource == playerIndex && msg.Payload.GetField("playerId").str.Equals(playerId)) {
			if (msg.Payload.HasField("direction")) {
				Debug.Log ("got direction: " + msg.Payload.GetField ("direction").i.ToString());
				int direction = msg.Payload.GetField ("direction").i;
				rb.AddForce((Vector3.right * (float)direction ) / 10.0f);


				Debug.Log("direction string: " + msg.Payload.GetField("direction").ToString ());

			}

			Debug.Log ("did not get angle, fields: " + msg.Payload.ToString ());
		}
	}

	void Attack(ControllerMessage msg) {
		Debug.Log ("got attack listen event");
		if (msg.ControllerSource == playerIndex && msg.Payload.GetField("playerId").str.Equals(playerId))
        {
            //hit someone
			if (msg.Payload.HasField("attack")) {
				Debug.Log ("doing an attack!");

				//eventually kill this
				rb.AddForce(Vector3.right * 20.0f);

				ShootAttackVector (transform.right);	//should throw raycast to the sprite's forward direction




			}
        }
	}

	void ShootAttackVector(Vector3 direction) {
		RaycastHit hit;
		Debug.DrawLine (transform.position, direction);

		if (Physics.Raycast (transform.position, direction, attackRange, out hit)) {
			
			if (hit.transform.tag == "Player") {
				Player hitPlayer = hit.rigidbody.gameObject.GetComponent<Player> ();
				if (hitPlayer.GetHit (transform.position)) {	//if we killed them
					myPoints++;
					GameManager.Instance.CheckPlayersWinCondition ();
				}
			}
		}
	}

	//returns true if player died
	public bool GetHit(Vector3 originPoint) {
		rb.AddForce ((transform.position - originPoint) * 100.0f);	//shoots player in the direction of the hit
		rb.AddForce(Vector3.up * 100.0f);
		initialHealth -= 10;
		if (initialHealth <= 0)
			return true;
		else
			return false;
	}

	void Jump(ControllerMessage msg) {
		//Debug.Log ("got jump listen event");
		//Debug.Log (msg.ControllerSource.ToString () + ", is the contrl source, the playerid match?: " + msg.Payload.GetField ("playerId").str + ":" + playerId);

		if (msg.ControllerSource == playerIndex && msg.Payload.GetField("playerId").str.Equals(playerId))
		{
			//Debug.Log ("match");
			if (msg.Payload.HasField("jump")) {
				Debug.Log ("doing a jump");
				rb.AddForce(Vector3.up * 100.0f);
			}
		}
	}


    
}
