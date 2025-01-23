using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public GameObject Player;

	void Start(){
		Player = GameObject.Find ("Player");
	}

	void Update () {
		transform.position += transform.right * 0.1f;
	}

	void OnTriggerEnter2D(Collider2D other) {
		Player.SendMessage ("HitEnemy");
		Destroy(other.gameObject);
		Destroy(this.gameObject);
	}
	//enemy Hit > Enemy Destroy > SendMessage > LevelUp
}
