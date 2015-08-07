using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {

	public GameObject titleGUICanvas;
	public GameObject gameGUICanvas;
	public GameObject gameOverGUICanvas;
	public GameObject invincibilityLayer;

	public Text topDistanceText;

	public Text healthText;
	public Text distanceText;

	public Text newRecordText;
	public Text lastDistanceText;

	public Image invincibilityImage;
	public Text invincibilityText;

	private CanvasGroup titleCG;
	private CanvasGroup gameCG;
	private CanvasGroup gameOverCG;
	
	public float transitionSpeed = 1.0f;

	private GameLogic.GameState currentState;

	void Start() {
		titleCG = titleGUICanvas.GetComponent<CanvasGroup>();
		gameCG = gameGUICanvas.GetComponent<CanvasGroup>();
		gameOverCG = gameOverGUICanvas.GetComponent<CanvasGroup>();
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
		titleGUICanvas.SetActive(true);
		gameGUICanvas.SetActive(false);
		gameOverGUICanvas.SetActive(false);
		invincibilityLayer.SetActive(false);
		//topScoreText.text = HighscoreController.GetTopScore().ToString("N0");
		topDistanceText.text = HighscoreController.GetTopDistance().ToString("0.00");
	}

	public void SwitchToGame() {
		SetAllTransparent();
		currentState = GameLogic.GameState.Game;
		titleGUICanvas.SetActive(false);
		gameGUICanvas.SetActive(true);
		gameOverGUICanvas.SetActive(false);
		invincibilityLayer.SetActive(false);
	}

	public void SwitchToGameOver(bool newRecord, float lastDistance) {
		SetAllTransparent();
		currentState = GameLogic.GameState.GameOver;
		titleGUICanvas.SetActive(false);
		gameGUICanvas.SetActive(false);
		gameOverGUICanvas.SetActive(true);
		invincibilityLayer.SetActive(false);
		if (newRecord) {
			Color oldColor = newRecordText.color;
			newRecordText.color = new Color(oldColor.r,oldColor.g,oldColor.b, 1.0f);
		}
		else {
			Color oldColor = newRecordText.color;
			newRecordText.color = new Color(oldColor.r,oldColor.g,oldColor.b, 0.0f);
		}
		//lastScoreText.text = lastScore.ToString("N0");
		lastDistanceText.text = lastDistance.ToString("0.00");
	}

	public void ShowInvincibility() {
		invincibilityLayer.SetActive(true);
	}

	public void HideInvincibility() {
		invincibilityLayer.SetActive(false);
		// TODO: play sound;
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
