using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public float scrollSpeed;
	public float interval;
	public float limit;

	private float distance;
	private float spawn;

	public GameObject[] backgroundSprites;

	// Use this for initialization
	void Start () {
		spawn = limit + 4 * interval;
	}
	
	// Update is called once per frame
	void Update () {
		distance = scrollSpeed * Time.deltaTime;
		//Debug.Log(spawn);

		foreach (GameObject bg in backgroundSprites) 
		{
			bg.transform.Translate(Vector3.down * distance, Space.World);
			if (bg.transform.position.y < limit) {
				bg.transform.position = new Vector3(bg.transform.position.x, spawn, bg.transform.position.z);
				Debug.Log(bg.transform.position.y);
			}
		}

	}
}
