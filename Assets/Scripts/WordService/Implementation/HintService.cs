using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HintService : IHintService
{
    List<string> charList = new List<string>();
    public void HintClick(Action<bool, List<Card>> callbackHint)
    {
       var allOpenCards = CardManager.instance.allFaceUpCards;
        charList.Clear();
        foreach (var card in allOpenCards) {
            charList.Add(card.cardData.letterText.text);
        }

        Debug.Log(charList.Count);
        if(GenerateWords(charList.ToArray(), 2, out string foundValidWorld))
        {
            List<Card> cards = new List<Card>();
            Debug.Log($"hello valid word found please high light card {foundValidWorld}");
            for (int i = 0; i < foundValidWorld.Length; i++)
            {
              var card =   allOpenCards.Find(objCard => objCard.cardData.letterText.text.ToLower() == foundValidWorld[i].ToString().ToLower());
              cards.Add(card);
            }
            Debug.Log(cards.Count);
            callbackHint.Invoke(true, cards);
            
        }
        else
        {
            callbackHint.Invoke(false, null);
            Debug.Log("No word is valid open Extra card");
        }
    }


    public bool GenerateWords(string[] chars, int length,  out string foundWord )
    {
        int totalWords = (int)Math.Pow(chars.Length, length);
       Debug.Log($"Total words possible: {totalWords}");

        for (int i = 0; i < chars.Length; i++)
        {
            for (int j = 0; j < chars.Length; j++)
            {
                string word  = chars[i]+ chars[j];
               
                if(WordValidator.instance.isWordValid(word))
                {
                    foundWord = word;
                    return true;
                }
            }
        }
        foundWord = null;
        return false;
    }

    public void OnWildClick(Card wildCard, string slotString)
    {
        throw new NotImplementedException();
    }
}
