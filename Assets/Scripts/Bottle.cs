using UnityEngine;
using System.Collections;

public class Bottle : MonoBehaviour {

	private float distance;
	
	// Update is called once per frame
	void Update () {
		distance = GameLogic.objectSpeed * Time.deltaTime;
		transform.Translate(Vector3.down * distance, Space.World);
		if (this.transform.position.y < -7.0f) {
			Destroy(gameObject);
		}
	}
}
