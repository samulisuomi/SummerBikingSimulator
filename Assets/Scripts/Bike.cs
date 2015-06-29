using UnityEngine;
using System.Collections;

public class Bike : MonoBehaviour {

	public float speed;
	public float xLimit;

	private float distance;

	// Update is called once per frame
	void Update () {
		if (transform.position.x > -xLimit) {
			// Keyboard:
			if (Input.GetKey("a")) {
				MoveLeft(speed * Time.deltaTime);
			}
			// Touch:
			if (Input.touchCount > 0) {
				Touch lastTouch = Input.touches[Input.touches.Length - 1];
				if (lastTouch.position.x < Screen.width / 2) {
					MoveLeft(speed * Time.deltaTime);
				}
			}
		}
		if (transform.position.x < xLimit) {
			// Keyboard:
			if (Input.GetKey("d")) {
				MoveRight(speed * Time.deltaTime);
			}
			// Touch:
			if (Input.touchCount > 0) {
				Touch lastTouch = Input.touches[Input.touches.Length - 1];
				if (lastTouch.position.x > Screen.width / 2) {
					MoveRight(speed * Time.deltaTime);
				}
			}
		}
	}

	void MoveLeft(float distance) {
		transform.Translate(Vector3.left * distance, Space.World);
	}

	void MoveRight(float distance) {
		transform.Translate(Vector3.right * distance, Space.World);
	}
}
