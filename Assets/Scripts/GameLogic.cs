using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Setup:
	public Bike bikeInstance;
	public Background backgroundInstance;

	public float startSpeed;
	public float maxSpeed;
	public float speedIncrease;
	public float speedIncreaseIntervalInSeconds;

	// Flags:
	public bool invincibility;

	// Private variables:
	private float startTime;
	private float intervalStartTime;
	private float intervalCounter;


	// States of the game:
	private enum GameState {
		Title,					// Beginning state: high score is shown
		Game,					//
		GameOver				//
	}
	private GameState gameState;

	// Use this for initialization
	void Start () {
		BeginGame();
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}

		// Other updates:
		if (gameState == GameState.Title) {
			// Wait for tap.
		}

		if (gameState == GameState.Game) {
			// Interval counting:
			intervalCounter = Time.time - intervalStartTime;
			if (intervalCounter > speedIncreaseIntervalInSeconds) {
				backgroundInstance.scrollSpeed += speedIncrease;
				intervalStartTime = Time.time;
				Debug.Log("New background scroll speed: " + backgroundInstance.scrollSpeed);
			}

		}

		if (gameState == GameState.GameOver) {
			// Wait for tap.
		}

	}

	void BeginTitle() {
		this.gameState = GameState.Title;
	}

	void BeginGame() {
		startTime = Time.time;
		intervalStartTime = startTime;
		bikeInstance.ResetBike();
		backgroundInstance.scrollSpeed = startSpeed;
		this.gameState = GameState.Game;
	}

}
