using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public float scrollSpeed;
	public float interval;
	public float limit;

	private float distance;
	private float spawn;

	public float totalDistance;

	public GameObject[] backgroundSprites;

	// Use this for initialization
	void Start () {
		ResetTotalDistance();
		spawn = limit + 4 * interval;
	}
	
	// Update is called once per frame
	void Update () {
		distance = scrollSpeed * Time.deltaTime;
		totalDistance = totalDistance + distance;

		foreach (GameObject bg in backgroundSprites) 
		{
			bg.transform.Translate(Vector3.down * distance, Space.World);
			if (bg.transform.position.y < limit) {
				float newY = bg.transform.position.y + 4 * interval;
				bg.transform.position = new Vector3(bg.transform.position.x, newY, bg.transform.position.z);
			}
		}

	}

	public void ResetTotalDistance() {
		totalDistance = 0.0f;
	}


}
