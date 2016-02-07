using UnityEngine;
using System.Collections;

public class replayPowerUp : MonoBehaviour {

	public bool isDoing = true;


	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && isDoing) {
			//Debug.Log ("replay attack triggered");
			GameManager.Instance.ExecuteReplayPowerUp (other.gameObject.GetComponent<Player> ());
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
