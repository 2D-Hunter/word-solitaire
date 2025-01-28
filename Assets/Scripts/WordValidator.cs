using System.Collections.Generic;
using UnityEngine;
using Word;

public class WordValidator : MonoBehaviour
{
    public static WordValidator instance;
    private HashSet<string> validWords;

    void Start()
    {
        instance = this;
        LoadDictionary();
    }

    private void LoadDictionary()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("Words");
        if (wordFile == null)
        {
            Debug.LogError("Word list not found!");
            return;
        }

        string[] words = wordFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        validWords = new HashSet<string>(words);
    }

    public bool ValidateWord(string word)
    {
        string createdWord = SlotManager.instance.GetSlotString();
        if (word.Length <= 1)
        {
            GameManager.instance.isValidWord = false;
            GreenTabHandler.instance.HandleGreenTab(createdWord);
            SubmitButton.instance.SwapImage();
            DictionaryButton.instance.SwapImage();
            return false;
        }
        
        GameManager.instance.isValidWord = WordServiceContainer.DictionaryService.isValidWord(word.ToLower());
        Debug.Log("_________________isValidWord: " + GameManager.instance.isValidWord);
        GreenTabHandler.instance.HandleGreenTab(createdWord);
        SubmitButton.instance.SwapImage();
        DictionaryButton.instance.SwapImage();
        return WordServiceContainer.DictionaryService.isValidWord(word.ToLower());
         //validWords.Contains(word.ToLower());
    }

    public bool isWordValid(string word)
    {
      return WordServiceContainer.DictionaryService.isValidWord(word.ToLower());
    }
}