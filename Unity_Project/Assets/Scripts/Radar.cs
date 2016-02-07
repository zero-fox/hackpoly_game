using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour {

    public GameObject target;
    public GameObject ship;
    public float radarDist = 2.0f;
    public float damping = 1.0f;

    // Use this for initialization
    void Start () {
        //Color color = target.GetComponent<Renderer>().material.color;
        //GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
	}

    // Update is called once per frame
    void LateUpdate() {
        if (target != null && ship != null)
        {
            Vector3 dir = target.transform.position - ship.transform.position;
            Vector3 radarPosition = (dir.normalized * radarDist) + ship.transform.position;
            transform.position = radarPosition;

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ + 270);
        }
    }
}
