using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthNumberGUI : MonoBehaviour {
	public Color full;
	public Color half;
	public Color oneLeft;

	private Text textComponent;

	void Start() {
		textComponent = GetComponentInParent<Text>();
		textComponent.color = half;
	}

	void Update() {
		if (textComponent.text == "3") {
			textComponent.color = full;
		} else if (textComponent.text == "2") {
			textComponent.color = half;
		} else if (textComponent.text == "1") {
			textComponent.color = oneLeft;
		}
	}
}
