using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }
    }
}
