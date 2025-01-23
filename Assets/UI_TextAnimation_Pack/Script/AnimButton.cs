using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class AnimButton : MonoBehaviour {

	public GameObject AnimGameObj;
	public GameObject GaneratePoint;

	public void Awake() {
		GaneratePoint = GameObject.Find ("GaneratePoint");
	}

	public void OnClick() {
		//GeneratePoint Child GameObject Destroy
		if (GaneratePoint.transform.childCount >= 1) {
			foreach (Transform n in GaneratePoint.transform) {
				GameObject.Destroy (n.gameObject);
			}
		}
			
		GameObject obj = Instantiate(AnimGameObj);
		obj.transform.parent = GaneratePoint.transform;
		obj.transform.localScale = new Vector3 (1, 1, 1);
	}

}
