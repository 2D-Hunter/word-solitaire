using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Word
{
    public class WordDictionaryService : IWordDictionaryService
    {
        public class WordInfo
        {
            public List<WordStartWithA> wordStartWithA { get; set; }
            public List<WordStartWithB> wordStartWithB { get; set; }
        }

        public class WordStartWithA
        {
            public int wordlength { get; set; }
            public bool isRemote { get; set; }
            public string fileName { get; set; }
        }

        public class WordStartWithB
        {
            public int wordlength { get; set; }
            public bool isRemote { get; set; }
            public string fileName { get; set; }
        }
        public void Initialize()
        {
            TextAsset txtTableInfo = Resources.Load<TextAsset>("TableInfo");

            if (txtTableInfo != null)
            {
                // Access the content of the file
                string fileContent = txtTableInfo.text;
                WordInfo TablesInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WordInfo>(fileContent);
                Debug.Log("File Content: " + fileContent);
            }
            else
            {
                Debug.LogError("TableInfo file not found in Resources folder!");
            }
        }
    }
}
