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
		if (invincibility) {
			// Do invincibility effect

			// TODO: Collision with enemy:
			if (false) {
				score += scoreDestroyEnemy;
			}
		}
		// TODO:Collision with sunglasses:
		if (false) {
			invincibility = true;
		}
		// TODO: Collision with bottle:
		if (false) {
			if (health < maxHealth) {
				health++;
			}
		}
	}
}
