using UnityEngine;
using System.Collections;

public class Bike : MonoBehaviour {

	public float speed;
	public float xLimit;

	public float startPositionX = 0.0f;

	private float distance;

	// Update is called once per frame
	void Update () {
		if (GameLogic.gameState == GameLogic.GameState.Game) {
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
	}

	void MoveLeft(float distance) {
		transform.Translate(Vector3.left * distance, Space.World);
	}

	void MoveRight(float distance) {
		transform.Translate(Vector3.right * distance, Space.World);
	}

	public void ResetBike() {
		transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
	}
}
