using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHintService 
{
    public void HintClick(Action<bool, List<Card>> callbackHint);
    public void OnWildClick(Card wildCard, string slotString);
   
}
