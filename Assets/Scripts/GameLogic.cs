using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameLogic : MonoBehaviour {

	public static float backgroundSpeed;
	public static float objectSpeed;

	// Setup:
	public Bike bikeInstance;
	public Helmet helmetInstance;
	public Background backgroundInstance;

	public Enemy enemyPrefab;
	public Bottle bottlePrefab;
	public Sunglasses sunglassesPrefab;

	public float bikeStartSpeed;	
	public float backgroundStartSpeed;
	public float backgroundMaxSpeed;
	public float speedIncrease;
	public float speedIncreaseIntervalInSeconds;
	public int startHealth;
	public float invincibilityLength;
	public float objectStartSpeed;
	public float objectIntervalDistance;

	public GameObject[] spawns;
	public GUIController guiControllerInstance;

	// Flags:

	// Private variables:
	private float gameStartTime;
	private float backgroundIntervalStartTime;
	private float backgroundIntervalCounter;

	private float objectIntervalStartDistance;
	private float objectIntervalDistanceCounter;

	private static float DISTANCE_SCALE = 1.81818181f;
	private static int TOTAL_SPAWNS = 3;

	private SpawnObject[] nextSpawns;

	private static string[][] spawnCombinations = new string[][] {
		new string[] {"110", "100"},
		new string[] {"100", "110"},
		new string[] {"011", "001"},
		new string[] {"001", "011"},
		new string[] {"001", "001"},
		new string[] {"100", "100"},
		new string[] {"100", "110"},
		new string[] {"001", "011"},
		new string[] {"011", "010", "010"},
		new string[] {"110", "010", "010"},
		new string[] {"110", "100", "100"},
		new string[] {"011", "001", "001"},
		new string[] {"011", "001", "001", "001", "010"},
		new string[] {"110", "100", "100", "100", "010"},
		new string[] {"001", "010"},
		new string[] {"100", "010"},
		new string[] {"100", "010"},
		new string[] {"101", "100", "110", "010", "010", "000", "011", "010", "010"},
		new string[] {"101", "100", "110", "100", "101", "001", "011", "011", "001", "001", "100"},
		new string[] {"101", "001", "011", "001", "101", "100", "110", "110", "100", "100", "001"},
		new string[] {"110", "000", "011", "010"},
		new string[] {"011", "000", "110", "010"},
		new string[] {"000"} // this needs to be the last one on the list
	};
	private int currentSpawnCombination;
	private int spawnIndexCounter;
	private int rowCounter;
	private int nextBottleSpawn;
	private int nextBottleDraw;
	private static int BOTTLE_ROW_INTERVAL = 20;
	private int nextSunglassesSpawn;
	private int nextSunglassesDraw;
	private static int SUNGLASSES_ROW_INTERVAL = 40;

	// States of the game:
	public enum GameState {
		Title,
		Game,
		GameOver,
		GameOverNewRecord
	}
	private GameState gameState;

	// States of the game:
	private enum SpawnObject {
		Enemy,
		Bottle,
		Sunglasses,
		Empty
	}
	
	// Use this for initialization
	void Start () {
		helmetInstance.maxHealth = startHealth;
		BeginTitle();
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}

		// Other updates:
		if (gameState == GameState.Title) {
			bikeInstance.ResetBike();

			if ((Input.touchCount > 0) || Input.GetKey("space")) {
				BeginGame();
			}
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
				PopulateNextSpawns();
				ExecuteSpawns();

				objectIntervalStartDistance = backgroundInstance.totalDistance;
			}

			// Update UI:
			// hudsomething.distance = (backgroundInstance.totalDistance * distanceScale).ToString("0.00"); 
			guiControllerInstance.healthText.text = helmetInstance.health + "";

			// Detect lives:
			if (helmetInstance.health <= 0) {
				BeginGameOver();
			}

		}

		if (gameState == GameState.GameOver) {
			if ((Input.touchCount > 0) || Input.GetKey("space")) {
				BeginTitle();
			}
		}

	}

	void BeginTitle() {
		bikeInstance.speed = 0.0f;
		backgroundSpeed = 0.0f;
		objectSpeed = 0.0f;
		backgroundInstance.ResetTotalDistance();

		// Destroy everything left from last game:
		var enemies = GameObject.FindGameObjectsWithTag("Enemy");
		var bottles = GameObject.FindGameObjectsWithTag("Bottle");
		var sunglasses = GameObject.FindGameObjectsWithTag("Sunglasses");
		
		foreach(var item in enemies) {
			Destroy(item);
		}

		foreach(var item in bottles) {
			Destroy(item);
		}

		foreach(var item in sunglasses) {
			Destroy(item);
		}

		this.gameState = GameState.Title;
		guiControllerInstance.SwitchToTitle();
	}

	void BeginGame() {
		bikeInstance.speed = bikeStartSpeed;
		gameStartTime = Time.time;
		backgroundIntervalStartTime = gameStartTime;
		bikeInstance.ResetBike();
		helmetInstance.health = startHealth;
		backgroundSpeed = backgroundStartSpeed;
		objectSpeed = objectStartSpeed;
		objectIntervalStartDistance = backgroundInstance.totalDistance;
		DrawNewSpawnCombination();
		rowCounter = 0;
		DrawNextBottleInterval();
		DrawNextSunglassesInterval();
		this.gameState = GameState.Game;
		guiControllerInstance.SwitchToGame();
	}

	void BeginGameOver() {
		bikeInstance.speed = 0.0f;
		backgroundSpeed = 0.0f;
		objectSpeed = 0.0f;
		this.gameState = GameState.GameOver;
		// new record?
		guiControllerInstance.SwitchToGameOver(false);
	}

	void ClearNextSpawns() {
		nextSpawns =  new SpawnObject[3] {SpawnObject.Empty, SpawnObject.Empty, SpawnObject.Empty};
	}

	void DrawNewSpawnCombination() {
		if (currentSpawnCombination == spawnCombinations.Length - 1) { // "000"
			currentSpawnCombination = Random.Range(0, spawnCombinations.Length - 1);
			Debug.Log("estettiin tyhjä");
		}
		else {
			currentSpawnCombination = Random.Range(0, spawnCombinations.Length);
		}
		spawnIndexCounter = 0;
	}

	void PopulateNextSpawns() {
		int nonEnemySlots = 0;
		int randomNonEnemySpawn = 0;
		rowCounter++;
		if (nextBottleDraw == rowCounter) {
			DrawNextBottleInterval();
		}
		if (nextSunglassesDraw == rowCounter) {
			DrawNextSunglassesInterval();
		}
		ClearNextSpawns();
		if (spawnIndexCounter < spawnCombinations[currentSpawnCombination].Length) {
			nonEnemySlots = spawnCombinations[currentSpawnCombination][spawnIndexCounter].Split('0').Length - 1;
			randomNonEnemySpawn = Random.Range(0, nonEnemySlots);
			for (int i = 0; i < 3; i++) {
				if (spawnCombinations[currentSpawnCombination][spawnIndexCounter][i] == '1') {
					nextSpawns[i] = SpawnObject.Enemy;
					randomNonEnemySpawn++;
				}
				else {
					if (IsBottleOnThisRow() && (i == randomNonEnemySpawn)) {
						nextSpawns[i] = SpawnObject.Bottle;
					} else if (IsSunglassesOnThisRow() && (i == randomNonEnemySpawn)) {
						nextSpawns[i] = SpawnObject.Sunglasses;
					}
				}
			}
			spawnIndexCounter++;
		} else {
			DrawNewSpawnCombination(); // there will be one line of no enemies
			if (IsBottleOnThisRow()) {
				nextSpawns[Random.Range(0,3)] = SpawnObject.Bottle;
			} else if (IsSunglassesOnThisRow()) {
				nextSpawns[Random.Range(0,3)] = SpawnObject.Sunglasses;
			}

		}
	}

	bool IsBottleOnThisRow() {
		if (nextBottleSpawn == rowCounter) {
			return true;
		}
		else {
			return false;
		}
	}

	bool IsSunglassesOnThisRow() {
		if (nextSunglassesSpawn == rowCounter) {
			return true;
		}
		else {
			return false;
		}
	}
	void DrawNextBottleInterval() {
		int spawnRow = Random.Range(0, BOTTLE_ROW_INTERVAL);
		nextBottleSpawn = rowCounter + spawnRow;
		nextBottleDraw = rowCounter + BOTTLE_ROW_INTERVAL;
		Debug.Log("New bottle drawn at rowCounter == " + rowCounter + " | spawnRow == " + spawnRow + " , nextBottleSpawn == " + nextBottleSpawn + " , nextBottleDraw == " + nextBottleDraw);
	}

	void DrawNextSunglassesInterval() {
		int spawnRow = Random.Range(0, SUNGLASSES_ROW_INTERVAL);
		nextSunglassesSpawn = rowCounter + spawnRow;
		nextSunglassesDraw = rowCounter + SUNGLASSES_ROW_INTERVAL;
		Debug.Log("New sunglasses drawn at rowCounter == " + rowCounter + " | spawnRow == " + spawnRow + " , nextSunglassesSpawn == " + nextSunglassesSpawn + " , nextSunglassesDraw == " + nextSunglassesDraw);
	}

	void ExecuteSpawns() {
		for (int i = 0; i < 3; i++) {
			if (nextSpawns[i] == SpawnObject.Enemy) {
				SpawnEnemy(i);
			}
			else if (nextSpawns[i] == SpawnObject.Bottle) {
				SpawnBottle(i);
			}
			else if (nextSpawns[i] == SpawnObject.Sunglasses) {
				SpawnSunglasses(i);
			}
			//Debug.Log("Spawn executed. currentSpawnCombination == " + currentSpawnCombination + ", spawnIndexCounter == " + spawnIndexCounter);
		}
	}

	void SpawnEnemy(int i) {
		if (spawns.Length == TOTAL_SPAWNS) {
			Instantiate(enemyPrefab, spawns[i].transform.position, Quaternion.identity);
		}
		else {
			Debug.Log("Warning: Number of spawn does not equal " + TOTAL_SPAWNS);
		}
	}
	void SpawnBottle(int i) {
		if (spawns.Length == TOTAL_SPAWNS) {
			Instantiate(bottlePrefab, spawns[i].transform.position, Quaternion.identity);
		}
		else {
			Debug.Log("Warning: Number of spawn does not equal " + TOTAL_SPAWNS);
		}
	}
	void SpawnSunglasses(int i) {
		if (spawns.Length == TOTAL_SPAWNS) {
			Instantiate(sunglassesPrefab, spawns[i].transform.position, Quaternion.identity);
		}
		else {
			Debug.Log("Warning: Number of spawn does not equal " + TOTAL_SPAWNS);
		}
	}
	

}
