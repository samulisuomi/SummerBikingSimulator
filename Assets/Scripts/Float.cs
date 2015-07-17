using UnityEngine;
using System.Collections;

public class Float : MonoBehaviour {

    public float amount = 1;
    public float speed = 1;

    private Vector3 startPosition;
	private Vector3 startScale;
	private float time;

    void Start()
    {
        startPosition = transform.localPosition;
		time = 0;
    }

	// Update is called once per frame
	void Update () {
		time = time + Time.deltaTime;
        transform.localPosition = new Vector3(startPosition.x, startPosition.y + amount * Mathf.Sin(2 * Mathf.PI * time * speed), startPosition.z);
	}
}
