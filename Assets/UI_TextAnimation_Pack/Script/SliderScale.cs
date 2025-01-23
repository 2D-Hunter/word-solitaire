using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SliderScale: MonoBehaviour {

	Slider _slider;

	public GameObject GaneratePoint_scale;

	void Start () {
		_slider = GameObject.Find("Slider").GetComponent<Slider>();
		GaneratePoint_scale = GameObject.Find ("GaneratePoint");
	}
	
	// Update is called once per frame
	void Update () {
		GaneratePoint_scale.transform.localScale = new Vector3 (_slider.value, _slider.value, _slider.value);
	}
}
