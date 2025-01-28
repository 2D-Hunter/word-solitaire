using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static Word.WordDictionaryService;

namespace Word
{
    public class WordDictionaryService : IWordDictionaryService
    {
        public class WordInfo
        {
            public int wordlength { get; set; }
            public bool isRemote { get; set; }
            public string fileName { get; set; }
        }

        public class WordInfoTable
        {
            public List<WordInfo> wordInfo { get; set; }
        }
        Dictionary<int , HashSet<string>> wordDictionary = new Dictionary<int, HashSet<string>>();

        public void Initialize()
        {
            TextAsset txtTableInfo = Resources.Load<TextAsset>("TableInfo");

            if (txtTableInfo != null)
            {
                // Access the content of the file
                string fileContent = txtTableInfo.text;
                WordInfoTable TablesInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WordInfoTable>(fileContent);
                /*WordInfoTable wordInfoT = new WordInfoTable();
                List<WordInfo> wordInfo = new List<WordInfo>();
                wordInfoT.wordInfo = wordInfo;
                WordInfo wordInfo1 = new WordInfo();
                wordInfo.Add(wordInfo1);
                fileContent =  Newtonsoft.Json.JsonConvert.SerializeObject(wordInfoT);*/
               if (TablesInfo != null) { 
                    Debug.Log("File Content: " + fileContent);
                   PrepareDictionary(TablesInfo);
                }
               
            }
            else
            {
                Debug.LogError("TableInfo file not found in Resources folder!");
            }
        }


        private void PrepareDictionary(WordInfoTable TablesInfo)
        {
            var wordInfo = TablesInfo.wordInfo;
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>" + wordInfo.ToString());
            foreach (var item in wordInfo)
            {
                TextAsset wordFile = Resources.Load<TextAsset>(item.fileName);
                if (wordFile == null)
                {
                    Debug.LogError("Word list not found!");
                    return;
                }

                string[] words = wordFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                HashSet<string> validWords = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
                wordDictionary.Add(item.wordlength, validWords);
            }
        }

        public bool isValidWord(string word)
        {
            int wordlength = word.Length;
            if(wordDictionary.ContainsKey(wordlength))
            {
                var words = wordDictionary[wordlength];
                Debug.Log("Word " + word);
                Debug.Log("Word length " + word.Length);
                return words.Contains(word);
            }

            return false;
        }

        public List<string> FindMatches(string pattern)
        {
            // Normalize pattern to lowercase
            pattern = pattern.ToLower();

            // Ensure the pattern is exactly 4 characters


            // Convert pattern to a regular expression
            string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".") + "$";

            // Filter words matching the pattern and with length 4
            List<string> matchingWords = new List<string>();
            HashSet<string> words = null;
            if (wordDictionary.ContainsKey(pattern.Length))
            {
                words = wordDictionary[pattern.Length];
                foreach (var word in words)
                {
                    if (Regex.IsMatch(word.ToLower(), regexPattern))
                    {
                        matchingWords.Add(word);
                    }
                }
            }
           

            return matchingWords;
        }
    }
}
