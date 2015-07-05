using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Setup:
	public Bike bikeInstance;

	// Flags:
	public bool invincibility;

	// Use this for initialization
	void Start () {
		bikeInstance.ResetBike();
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}
	}
}
