
using System.ComponentModel.Design;
using UnityEngine;

namespace Word
{
    public class WordServiceContainer
    {
        private static IWordDictionaryService _dictionaryService;
        private static IWordMatchService _matchService;
        private static ILetterService _letterService;
        private static INetworkService _networkService;
        private static IHintService _hintService;
        public static IWordDictionaryService DictionaryService { get { return _dictionaryService; } }
        public static IWordMatchService MatchService {  get { return _matchService; } }
        public static ILetterService LetterService { get { return _letterService; } }
        public static INetworkService NetworkService { get { return _networkService; } }
        public static IHintService HintService { get { return _hintService; } }

        private WordServiceContainer() { }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeServicer()
        {
            _dictionaryService = new WordDictionaryService();
            _matchService = new WordMatchService();
           
            _networkService = new NetworkService();
            _hintService = new HintService();
            _dictionaryService.Initialize();

        }
    }
}
