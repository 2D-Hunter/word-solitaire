using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject GameStart;
	public GameObject Clear;
	public GameObject LevelUp;

	void Start(){
		MakeGameStart ();
	}


	//GameStart_Instance
	void MakeGameStart(){
		Instantiate (GameStart, new Vector3 (0, 2, 0), Quaternion.identity);
	}
		
	//Clear_Instance
	void MakeClear(){
		Instantiate (Clear, new Vector3 (0, 2, 0), Quaternion.identity);
	}

	//LevelUp_Instance
	void MakeLevelUp(){
		Instantiate (LevelUp, new Vector3 (0, 0, 0), Quaternion.identity);
	}

}
