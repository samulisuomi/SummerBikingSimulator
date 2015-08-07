using UnityEngine;
using System.Collections;

public class Helmet : MonoBehaviour {

	public int score;
	public int health;
	public int maxHealth;
	public float invincibilityLength;
	public float invincibilityCounter;
	public bool invincibility;

	public int scoreDestroyEnemy;

	public AudioClip damageSound1;
	public AudioClip damageSound2;
	public AudioClip destroyEnemySound1;
	public AudioClip destroyEnemySound2;
	public AudioClip waterSound1;
	public AudioClip waterSound2;
	public AudioClip invincibilityStartSound;

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
				SoundManager.instance.RandomiseSfx(damageSound1, damageSound2);
				Destroy(other.gameObject);
			}
			FireDestroyEnemyAnimation();
		}
		else if (other.tag == "Bottle") {
			Debug.Log("Bottle hit");
			if (health < maxHealth) {
				health++;
				SoundManager.instance.RandomiseSfx(waterSound1, waterSound2);
				Destroy(other.gameObject);
			}
		}
		else if (other.tag == "Sunglasses") {
			Debug.Log("Sunglasses hit");
			StartInvincibility();
			SoundManager.instance.PlaySingle(invincibilityStartSound);
			Destroy(other.gameObject);
		}
	}

	public void StartInvincibility() {
		// todo: show ui
		invincibility = true;
		invincibilityCounter = invincibilityLength;
	}

	void FireDestroyEnemyAnimation() {
		if (invincibility) {
		} else {
		}
	}
}
