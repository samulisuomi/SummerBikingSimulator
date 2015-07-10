using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	public static float backgroundSpeed;
	public static float objectSpeed;

	// Setup:
	public Bike bikeInstance;
	public Helmet helmetInstance;
	public Background backgroundInstance;

	public Enemy enemyPrefab;
	public Bottle bottlePrefab;

	public float backgroundStartSpeed;
	public float backgroundMaxSpeed;
	public float speedIncrease;
	public float speedIncreaseIntervalInSeconds;
	public int startHealth;
	public float invincibilityLength;
	public float objectStartSpeed;
	public float objectIntervalDistance;

	public GameObject[] spawns;

	// Flags:

	// Private variables:
	private float gameStartTime;
	private float backgroundIntervalStartTime;
	private float backgroundIntervalCounter;

	private float objectIntervalStartDistance;
	private float objectIntervalDistanceCounter;

	private float distanceScale = 1.81818181f;

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
			// Background speed increments:
			backgroundIntervalCounter = Time.time - backgroundIntervalStartTime;
			if ((backgroundIntervalCounter > speedIncreaseIntervalInSeconds) && (backgroundSpeed < backgroundMaxSpeed)) {
				backgroundSpeed += speedIncrease;
				backgroundIntervalStartTime = Time.time;
				Debug.Log("New background scroll speed: " + backgroundSpeed);
				objectSpeed += speedIncrease;
			}

			// Object spawn intervals:
			objectIntervalDistanceCounter = backgroundInstance.totalDistance - objectIntervalStartDistance;
			if (objectIntervalDistanceCounter > objectIntervalDistance) {
				SpawnEnemy(1); //TODO: switch to spawn control method

				if (Random.Range(0.0f, 1.0f) > 0.69f) {
					SpawnBottle(0);
				}

				objectIntervalStartDistance = backgroundInstance.totalDistance;
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
		backgroundSpeed = 0.0f;
		objectSpeed = 0.0f;
		backgroundInstance.ResetTotalDistance();
		this.gameState = GameState.Title;
	}

	void BeginGame() {
		gameStartTime = Time.time;
		backgroundIntervalStartTime = gameStartTime;
		bikeInstance.ResetBike();
		helmetInstance.health = startHealth;
		backgroundSpeed = backgroundStartSpeed;
		objectSpeed = objectStartSpeed;
		objectIntervalStartDistance = backgroundInstance.totalDistance;
		this.gameState = GameState.Game;
	}

	void BeginGameOver() {
		backgroundSpeed = 0.0f;
		objectSpeed = 0.0f;
		this.gameState = GameState.GameOver;
	}

	// TODO: spawn management and actual spawning should be in different methods
	void SpawnEnemy(int i) {
		if (spawns.Length == 3) {
			Instantiate(enemyPrefab, spawns[i].transform.position, Quaternion.identity);
		}
		else {
			Debug.Log("Warning: 3 spawns not set up");
		}
	}
	void SpawnBottle(int i) {
		if (spawns.Length == 3) {
			Instantiate(bottlePrefab, spawns[i].transform.position, Quaternion.identity);
		}
		else {
			Debug.Log("Warning: 3 spawns not set up");
		}
	}

}
