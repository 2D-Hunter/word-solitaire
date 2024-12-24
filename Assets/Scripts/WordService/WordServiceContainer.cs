
using UnityEngine;

namespace Word
{
    public class WordServiceContainer
    {
        private static IWordDictionaryService _dictionaryService;
        private static IWordMatchService _matchService;
        private static ILetterService _letterService;
        private static INetworkService _networkService;

        public static IWordDictionaryService DictionaryService { get { return _dictionaryService; } }
        public static IWordMatchService MatchService {  get { return _matchService; } }
        public static ILetterService LetterService { get { return _letterService; } }
        public static INetworkService NetworkService { get { return _networkService; } }

        private WordServiceContainer() { }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeServicer()
        {
            _dictionaryService = new WordDictionaryService();
            _matchService = new WordMatchService();
            _letterService  = new ILetterService();
            _networkService = new INetworkService();


            Debug.Log("Hello Service is create");
        }
    }
}
