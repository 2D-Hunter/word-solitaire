using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class CategoryButton : MonoBehaviour {

	private GameObject ButtonController;
	private GameObject panel_object;

	public string pushButtonName;

	public void Awake() {
		ButtonController = GameObject.Find ("ButtonController");
		panel_object = this.gameObject;
	}

	public void OnClick() {
		//Button Push Color Green
		panel_object.GetComponent<Image>().color = new Color(0, 1, 0, 1);

		//Push Button Excepting Color White
		pushButtonName = panel_object.name;
		ButtonController.SendMessage(pushButtonName);

	}

}
