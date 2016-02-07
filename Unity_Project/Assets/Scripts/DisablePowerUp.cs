using UnityEngine;
using System.Collections;

public class DisablePowerUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public bool isDoing = true;

	//research this

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && isDoing) {
			//Debug.Log ("disable attack triggered");
			StartCoroutine (GameManager.Instance.ExecuteDisablePlayersPowerUp (other.gameObject.GetComponent<Player> ()));
			GameManager.Instance.powerUpInPlay = false;

			//gameObject.SetActive (false);
			StartCoroutine (DestroyMe ());
			//Destroy (gameObject);
		}

	}
	public IEnumerator DestroyMe() {
		isDoing = false;
		yield return new WaitForSeconds (6.0f);
		Destroy (gameObject);
	}
}
