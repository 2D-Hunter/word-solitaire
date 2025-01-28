using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Word
{
    public interface IWordDictionaryService
    {
        void Initialize();
        bool isValidWord(string word);
        public List<string> FindMatches(string pattern);
    }

}
