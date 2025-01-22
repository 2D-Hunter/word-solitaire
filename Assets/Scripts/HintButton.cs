using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;

public class HintButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnclickHint()
    {
        WordServiceContainer.HintService.HintClick((isfound , cards) =>
        {
            if(isfound) {
                Debug.Log("high light " + cards.Count);
            }
            else
            {
                Debug.Log("Open Extra Hint ");
            }
        });
    }
}
