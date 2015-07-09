using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Setup:
	public Bike bikeInstance;
	public Helmet helmetInstance;
	public Background backgroundInstance;

	public float startSpeed;
	public float maxSpeed;
	public float speedIncrease;
	public float speedIncreaseIntervalInSeconds;
	public int startHealth;
	public float invincibilityLength;
	public float enemyStartSpeed;

	// Flags:

	// Private variables:
	private float startTime;
	private float intervalStartTime;
	private float intervalCounter;

	private int distanceScale = 2;

	// States of the game:
	private enum GameState {
		Title,					// Beginning state: high score is shown
		Game,					//
		GameOver				//
	}
	private GameState gameState;

	// Use this for initialization
	void Start () {
		helmetInstance.maxHealth = startHealth;
		BeginGame();
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}

		// Other updates:
		if (gameState == GameState.Title) {
			bikeInstance.ResetBike();

			// Wait for tap.
		}

		if (gameState == GameState.Game) {
			// Interval counting:
			intervalCounter = Time.time - intervalStartTime;
			if ((intervalCounter > speedIncreaseIntervalInSeconds) && (backgroundInstance.scrollSpeed < maxSpeed)) {
				backgroundInstance.scrollSpeed += speedIncrease;
				intervalStartTime = Time.time;
				Debug.Log("New background scroll speed: " + backgroundInstance.scrollSpeed);
				Enemy.speed += speedIncrease;
			}

			// Update UI:
			// hudsomething.distance = (backgroundInstance.totalDistance * distanceScale).ToString("0.00"); 
			// hudsomething.health = helmetInstance.health;

			// Detect lives:
			if (helmetInstance.health <= 0) {
				BeginGameOver();
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
		helmetInstance.health = startHealth;
		backgroundInstance.scrollSpeed = startSpeed;
		Enemy.speed = enemyStartSpeed;
		this.gameState = GameState.Game;
	}

	void BeginGameOver() {
		backgroundInstance.scrollSpeed = 0.0f;
		this.gameState = GameState.GameOver;
	}

}
