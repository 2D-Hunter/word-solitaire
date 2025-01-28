using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;

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
        Debug.Log("OnWildClick ");
        var slotCards = SlotManager.instance.slotsCard;
        string cardtext = null;
        string cardsChars = null;
        for (int i = 0; i<slotCards.Count; i++)
        {
            Card card = slotCards[i];
           
            
            if (card != null) {
                if(card.IsWildCard())
                {
                    cardtext = "*";
                }
                else
                {
                    cardtext = card.cardData.letterText.text;
                }
                cardsChars += cardtext;
            }
        }
        Debug.Log("cardsChars >>>>>>>>>>>>>>>>>> " + cardsChars);
        var matchWords =    WordServiceContainer.DictionaryService.FindMatches(cardsChars);
        if(matchWords != null && matchWords.Count>0)
        {
            var word = matchWords[0];
            int indexofWild = cardsChars.IndexOf("*");
            var cardChar = word[indexofWild];
            wildCard.cardData.letter = cardChar;
        }
        else
        {
           // wildCard.cardData.letter = "*";
        }
        /*foreach (string match in matchWords)
        {
            Debug.Log("Math found <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<"+match);
        }*/

    }
}
