using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;


public class GameLogic : MonoBehaviour {

	public static float backgroundSpeed;
	public static float objectSpeed;

	// Setup (TODO: this stuff should be in a separate place to keep things DRY):
	public Bike bikeInstance;
	public Helmet helmetInstance;
	public Background backgroundInstance;

	public Enemy enemyPrefab;
	public Bottle bottlePrefab;
	public Sunglasses sunglassesPrefab;

	public float bikeStartSpeed;	
	public float backgroundStartSpeed;
	public float backgroundHighDifficultySpeed;
	public float backgroundMaxSpeed;
	public float speedIncrease;
	public float speedIncreaseAfterHighDifficultySpeed;
	public float speedIncreaseIntervalInSeconds;
	public int startHealth;
	public float invincibilityLength;
	public float objectStartSpeed;
	public float objectIntervalDistance;

	public GameObject[] spawns;
	public GUIController guiControllerInstance;

	public AudioClip gameStartSound;
	public AudioClip newRecordSound;

	// Flags:

	// Private variables:
	private float gameStartTime;
	private float backgroundIntervalStartTime;
	private float backgroundIntervalCounter;

	private float objectIntervalStartDistance;
	private float objectIntervalDistanceCounter;

	private static float DISTANCE_SCALE = 0.3f;
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
	private static int SUNGLASSES_ROW_INTERVAL = 50;
	public float invincibilityCounter;
	private bool showInvincibilityCalledOnce; //BAD DESIGN!!!!!!!!
	private BannerView bannerView;

	// States of the game:
	public enum GameState {
		Title,
		Game,
		GameOver,
		GameOverNewRecord
	}
	public static GameState gameState;

	// States of the game:
	private enum SpawnObject {
		Enemy,
		Bottle,
		Sunglasses,
		Empty
	}

	private float gameOverUICounter;
	
	// Use this for initialization
	void Start () {
		helmetInstance.maxHealth = startHealth;
		BeginTitle();
	}

	void Awake() {
		RequestBanner();
		bannerView.Hide();
	}

	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}

		// Other updates:
		if (gameState == GameState.Title) {
			bikeInstance.ResetBike();

			if (Input.GetMouseButtonDown(0) || Input.GetKey("space")) {
				BeginGame();
			}
		}

		if (gameState == GameState.Game) {
			// Background speed increments:
			backgroundIntervalCounter = Time.time - backgroundIntervalStartTime;
			if ((backgroundIntervalCounter > speedIncreaseIntervalInSeconds) && (backgroundSpeed < backgroundMaxSpeed)) {
				if (backgroundSpeed < backgroundHighDifficultySpeed) {
					backgroundSpeed += speedIncrease;
				} else {
					backgroundSpeed += speedIncreaseAfterHighDifficultySpeed;
				}
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
			//guiControllerInstance.scoreText.text = (helmetInstance.score).ToString("N0"); 
			guiControllerInstance.distanceText.text = (backgroundInstance.totalDistance * DISTANCE_SCALE).ToString("0.00"); 
			guiControllerInstance.healthText.text = helmetInstance.health + "";

			// Detect lives:
			if (helmetInstance.health <= 0) {
				BeginGameOver();
			}

			// Invincibility:
			if (helmetInstance.invincibility) {
				if (!showInvincibilityCalledOnce) {
					guiControllerInstance.ShowInvincibility();
					showInvincibilityCalledOnce = true;
				}
				helmetInstance.invincibilityCounter -= Time.deltaTime;
				if (helmetInstance.invincibilityCounter < 0.0f) {
					EndInvcinbility();
				}
				guiControllerInstance.invincibilityTime.text = ((int) helmetInstance.invincibilityCounter + 1.0f) + "";
			}
		}

		if (gameState == GameState.GameOver) {
			if (gameOverUICounter < 0.7f) {
				gameOverUICounter += Time.deltaTime;
			} else {
				if (Input.GetMouseButtonDown(0) || Input.GetKey("space")) {
					BeginTitle();
				}
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

		gameState = GameState.Title;
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
		helmetInstance.invincibilityLength = invincibilityLength;
		helmetInstance.invincibilityCounter = 0.0f;
		DrawNextBottleInterval();
		DrawNextSunglassesInterval();
		gameState = GameState.Game;
		guiControllerInstance.SwitchToGame();
		bannerView.Hide();
		SoundManager.instance.PlaySingle(gameStartSound);
	}

	void BeginGameOver() {
		gameOverUICounter = 0.0f;
		bikeInstance.speed = 0.0f;
		backgroundSpeed = 0.0f;
		objectSpeed = 0.0f;
		gameState = GameState.GameOver;

		bool newRecord = false;

		if (HighscoreController.GetTopDistance() < (backgroundInstance.totalDistance * DISTANCE_SCALE)) {
			HighscoreController.SetTopDistance(backgroundInstance.totalDistance * DISTANCE_SCALE);
			newRecord = true;
			SoundManager.instance.PlaySingle(newRecordSound);
		}
		guiControllerInstance.SwitchToGameOver(newRecord, (backgroundInstance.totalDistance * DISTANCE_SCALE));
		bannerView.Show();
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

	void EndInvcinbility() {
		// todo: ui = 0 before the fade
		// todo: hide UI
		helmetInstance.invincibility = false;
		showInvincibilityCalledOnce = false;
		guiControllerInstance.HideInvincibility();
		SoundManager.instance.PlaySingle(helmetInstance.invincibilityEndSound);
	}

	void OnDestroy() {
		bannerView.Destroy();
	}

	private void RequestBanner()
	{
		#if UNITY_ANDROID
		string adUnitId = Secrets.AD_UNIT_ID;
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.TagForChildDirectedTreatment(true)
			.Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}
}
