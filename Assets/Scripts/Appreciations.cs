using UnityEngine;
using System.Collections.Generic;

public class Appreciations : MonoBehaviour
{
	public static Appreciations instance;
	//public List<GameObject> appreciations;
	public ParticleSystem confettiEffect;
	void Start()
	{
		instance = this;
	}

	public void ShowAppreciation()
	{
		//int randomIndex = Random.Range(0, appreciations.Count);
		//GameObject rnd = appreciations[randomIndex];
		//rnd.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		//GameObject instance = Instantiate(rnd, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
		//instance.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y, 0); // Ensure Z position is 0
		//instance.transform.SetParent(null);

		confettiEffect.Play();
	}
}
