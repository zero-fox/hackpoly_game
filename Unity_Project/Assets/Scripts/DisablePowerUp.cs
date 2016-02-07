using UnityEngine;
using System.Collections;

public class DisablePowerUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//research this

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			//alls game manager with the player as a parameter
			//broadcasts to HH -> if their name doesn tmatch it,
			//disable
			//game manager starts coroutine, resumes control after 3 seconds
			//sends message to HH to resume control

			//note: the listeners are all handled with game manager on TT side, not HH side


			Destroy (gameObject);
		}

	}
}
