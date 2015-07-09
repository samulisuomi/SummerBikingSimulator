using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public static float speed;

	private float distance;

	// Update is called once per frame
	void Update () {
		distance = speed * Time.deltaTime;
		transform.Translate(Vector3.down * distance, Space.World);
	}
}
