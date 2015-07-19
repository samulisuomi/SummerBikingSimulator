using UnityEngine;
using System.Collections;

public class HighscoreController : MonoBehaviour {

	public static int GetTopScore() {
		return PlayerPrefs.GetInt("topScore");
	}

	public static float GetTopDistance() {
		return PlayerPrefs.GetFloat("topDistance");
	}

	public static void SetTopScore(int score) {
		PlayerPrefs.SetInt("topScore", score);
	}

	public static void SetTopDistance(float distance) {
		PlayerPrefs.SetFloat("topDistance", distance);
	}
}
