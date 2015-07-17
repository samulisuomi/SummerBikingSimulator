using UnityEngine;
using System.Collections;

public class Helmet : MonoBehaviour {

	public int score;
	public int health;
	public int maxHealth;
	public bool invincibility;

	public int scoreDestroyEnemy;

	// Update is called once per frame
	void Update () {
		// TODO: Invincibility effect and countdown
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy") {
			if (invincibility) {
				Debug.Log("Enemy hit when invincible");
				score += scoreDestroyEnemy;
				Destroy(other.gameObject);
			} else {
				Debug.Log("Enemy hit when not invincible");
				health--;
				Destroy(other.gameObject);
			}
			FireDestroyEnemyAnimation();
		}
		else if (other.tag == "Bottle") {
			if (health < maxHealth) {
				health++;
			}
		}
		else if (other.tag == "Sunglasses") {
			Debug.Log("Sunglasses hit");
			StartInvincibility();
		}
	}

	void StartInvincibility() {
	}

	void FireDestroyEnemyAnimation() {
		if (invincibility) {
		} else {
		}
	}
}
