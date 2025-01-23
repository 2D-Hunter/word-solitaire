using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	void Update () {
		transform.position -= transform.right * 0.02f;
	}
}
