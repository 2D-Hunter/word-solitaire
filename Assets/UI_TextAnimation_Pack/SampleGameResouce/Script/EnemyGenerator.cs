using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {

	public GameObject Enemy;

	public float waitGenerateTime = 1.5f;

	private float timer = 0.0f;

	void Update () {
		timer += Time.deltaTime;
		if (timer > waitGenerateTime) {
			GameObject obj = (GameObject)Instantiate (Enemy, transform.position, transform.rotation);
			obj.transform.parent = transform;
			timer = 0.0f;
		}
	}
}
