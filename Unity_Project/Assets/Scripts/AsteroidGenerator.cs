using UnityEngine;
using System.Collections.Generic;

public class AsteroidGenerator : MonoBehaviour {

	public GameObject asteroidPrefab;
	public float radius;
	public int asteroids;
	public float minDistance;
	
	void Start() {
		Application.runInBackground = true;
		
		for(int i=0; i<asteroids; i++) {
			Vector2 loc = new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0.0f);
			
			Instantiate(asteroidPrefab, loc, Quaternion.identity);
		}
	}
}
