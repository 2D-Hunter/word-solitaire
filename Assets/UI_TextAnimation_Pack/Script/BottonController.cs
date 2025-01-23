using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class BottonController : MonoBehaviour {

	private GameObject CategoryButton1;
	private GameObject CategoryButton2;
	private GameObject CategoryButton3;
	private GameObject CategoryButton4;
	private GameObject CategoryButton5;
	private GameObject CategoryButton6;
	private GameObject CategoryButton7;
	private GameObject Anim_ButtonSet_Category1;
	private GameObject Anim_ButtonSet_Category2;
	private GameObject Anim_ButtonSet_Category3;
	private GameObject Anim_ButtonSet_Category4;
	private GameObject Anim_ButtonSet_Category5;
	private GameObject Anim_ButtonSet_Category6;
	private GameObject Anim_ButtonSet_Category7;

	public void Awake() {
		CategoryButton1 = transform.Find ("CategoryButton_Set/Category_Button1").gameObject;
		CategoryButton2 = transform.Find ("CategoryButton_Set/Category_Button2").gameObject;
		CategoryButton3 = transform.Find ("CategoryButton_Set/Category_Button3").gameObject;
		CategoryButton4 = transform.Find ("CategoryButton_Set/Category_Button4").gameObject;
		CategoryButton5 = transform.Find ("CategoryButton_Set/Category_Button5").gameObject;
		CategoryButton6 = transform.Find ("CategoryButton_Set/Category_Button6").gameObject;
		CategoryButton7 = transform.Find ("CategoryButton_Set/Category_Button7").gameObject;
		Anim_ButtonSet_Category1 = transform.Find ("Anim_ButtonSet_Category1").gameObject;
		Anim_ButtonSet_Category2 = transform.Find ("Anim_ButtonSet_Category2").gameObject;
		Anim_ButtonSet_Category3 = transform.Find ("Anim_ButtonSet_Category3").gameObject;
		Anim_ButtonSet_Category4 = transform.Find ("Anim_ButtonSet_Category4").gameObject;
		Anim_ButtonSet_Category5 = transform.Find ("Anim_ButtonSet_Category5").gameObject;
		Anim_ButtonSet_Category6 = transform.Find ("Anim_ButtonSet_Category6").gameObject;
		Anim_ButtonSet_Category7 = transform.Find ("Anim_ButtonSet_Category7").gameObject;
		Category_Button1 ();
		CategoryButton1.GetComponent<Image>().color = new Color(0, 1, 0, 1);
	}

	public void Category_Button1() {
		CategoryButton2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton4.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton7.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category1.active = true;
		Anim_ButtonSet_Category2.active = false;
		Anim_ButtonSet_Category3.active = false;
		Anim_ButtonSet_Category4.active = false;
		Anim_ButtonSet_Category5.active = false;
		Anim_ButtonSet_Category6.active = false;
		Anim_ButtonSet_Category7.active = false;
	}
	public void Category_Button2() {
		CategoryButton1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton4.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton7.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category2.active = true;
		Anim_ButtonSet_Category1.active = false;
		Anim_ButtonSet_Category3.active = false;
		Anim_ButtonSet_Category4.active = false;
		Anim_ButtonSet_Category5.active = false;
		Anim_ButtonSet_Category6.active = false;
		Anim_ButtonSet_Category7.active = false;
	}
	public void Category_Button3() {
		CategoryButton1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton4.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton7.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category3.active = true;
		Anim_ButtonSet_Category1.active = false;
		Anim_ButtonSet_Category2.active = false;
		Anim_ButtonSet_Category4.active = false;
		Anim_ButtonSet_Category5.active = false;
		Anim_ButtonSet_Category6.active = false;
		Anim_ButtonSet_Category7.active = false;

	}
	public void Category_Button4() {
		CategoryButton1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton7.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category4.active = true;
		Anim_ButtonSet_Category1.active = false;
		Anim_ButtonSet_Category2.active = false;
		Anim_ButtonSet_Category3.active = false;
		Anim_ButtonSet_Category5.active = false;
		Anim_ButtonSet_Category6.active = false;
		Anim_ButtonSet_Category7.active = false;

	}
	public void Category_Button5() {
		CategoryButton1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton4.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton7.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category5.active = true;
		Anim_ButtonSet_Category1.active = false;
		Anim_ButtonSet_Category2.active = false;
		Anim_ButtonSet_Category3.active = false;
		Anim_ButtonSet_Category4.active = false;
		Anim_ButtonSet_Category6.active = false;
		Anim_ButtonSet_Category7.active = false;

	}
	public void Category_Button6() {
		CategoryButton1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton4.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton7.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category6.active = true;
		Anim_ButtonSet_Category1.active = false;
		Anim_ButtonSet_Category2.active = false;
		Anim_ButtonSet_Category3.active = false;
		Anim_ButtonSet_Category4.active = false;
		Anim_ButtonSet_Category5.active = false;
		Anim_ButtonSet_Category7.active = false;
	}
	public void Category_Button7() {
		CategoryButton1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton4.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		CategoryButton6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Anim_ButtonSet_Category7.active = true;
		Anim_ButtonSet_Category1.active = false;
		Anim_ButtonSet_Category2.active = false;
		Anim_ButtonSet_Category3.active = false;
		Anim_ButtonSet_Category4.active = false;
		Anim_ButtonSet_Category5.active = false;
		Anim_ButtonSet_Category6.active = false;

	}
}
