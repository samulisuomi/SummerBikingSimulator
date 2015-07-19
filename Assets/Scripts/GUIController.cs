using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {

	public GameObject TitleGUICanvas;
	public GameObject GameGUICanvas;
	public GameObject GameOverGUICanvas;

	public Text topScoreText;
	public Text topDistanceText;

	public Text healthText;
	public Text scoreText;
	public Text distanceText;

	public Text newRecordText;
	public Text lastScoreText;
	public Text lastDistanceText;

	private CanvasGroup titleCG;
	private CanvasGroup gameCG;
	private CanvasGroup gameOverCG;
	
	public float transitionSpeed = 1.0f;

	private GameLogic.GameState currentState;

	void Start() {
		titleCG = TitleGUICanvas.GetComponent<CanvasGroup>();
		gameCG = GameGUICanvas.GetComponent<CanvasGroup>();
		gameOverCG = GameOverGUICanvas.GetComponent<CanvasGroup>();
	}

	void Update() {
		if (currentState == GameLogic.GameState.Title && titleCG.alpha < 1.0f) {
			RefreshAlpha(titleCG);
		}
		else if (currentState == GameLogic.GameState.Game && gameCG.alpha < 1.0f) {
			RefreshAlpha(gameCG);
		}
		else if (currentState == GameLogic.GameState.GameOver && gameOverCG.alpha < 1.0f) {
			RefreshAlpha(gameOverCG);
		}
	}

	public void SwitchToTitle() {
		SetAllTransparent();
		currentState = GameLogic.GameState.Title;
		TitleGUICanvas.SetActive(true);
		GameGUICanvas.SetActive(false);
		GameOverGUICanvas.SetActive(false);
		topScoreText.text = HighscoreController.GetTopScore().ToString("N0");
		topDistanceText.text = HighscoreController.GetTopDistance().ToString("0.00");
	}

	public void SwitchToGame() {
		SetAllTransparent();
		currentState = GameLogic.GameState.Game;
		TitleGUICanvas.SetActive(false);
		GameGUICanvas.SetActive(true);
		GameOverGUICanvas.SetActive(false);
	}

	public void SwitchToGameOver(bool newRecord, int lastScore, float lastDistance) {
		SetAllTransparent();
		currentState = GameLogic.GameState.GameOver;
		TitleGUICanvas.SetActive(false);
		GameGUICanvas.SetActive(false);
		GameOverGUICanvas.SetActive(true);
		if (newRecord) {
			Color oldColor = newRecordText.color;
			newRecordText.color = new Color(oldColor.r,oldColor.g,oldColor.b, 1.0f);
		}
		else {
			Color oldColor = newRecordText.color;
			newRecordText.color = new Color(oldColor.r,oldColor.g,oldColor.b, 0.0f);
		}
		lastScoreText.text = lastScore.ToString("N0");
		lastDistanceText.text = lastDistance.ToString("0.00");
	}

	public void RefreshAlpha(CanvasGroup cg) {
		float newAlpha = cg.alpha + transitionSpeed * Time.deltaTime;
		if (newAlpha < 1.0f) {
			cg.alpha = newAlpha;
		} else {
			cg.alpha = 1.0f;
		}
	}

	void SetAllTransparent() {
		titleCG.alpha = 0.0f;
		gameCG.alpha = 0.0f;
		gameOverCG.alpha = 0.0f;
	}

}
