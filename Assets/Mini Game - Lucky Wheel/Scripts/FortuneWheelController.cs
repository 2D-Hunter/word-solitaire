using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortuneWheelController : MonoBehaviour
{
    private void Awake()
    {
        Menu.instance.isFortuneWheelOpened = true;
    }
    private void Start()
    {
        Invoke("TurnWheel", 1f);
        
    }
    void TurnWheel()
    {
        FortuneWheel.GameController.ins.TurnWheel();
    }
    public void CloseThis()
    {
        Menu.instance.isFortuneWheelOpened = false;
        gameObject.SetActive(false);
    }
}
