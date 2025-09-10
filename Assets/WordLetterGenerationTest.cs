using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;

public class WordLetterGenerationTest : MonoBehaviour
{
    private LetterBucket letterBucket;
    bool Completed = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        StartCoroutine(WordLetterGenerationSystem.Instance.LoadWordLetterGenerationSystem());
        WordServiceContainer.NetworkService.GetGameData("https://2dhunter.s3.us-west-2.amazonaws.com/word-solitaire-go/fb/config/letterbucket.json", (issucess, data) =>
        {
            if (issucess)
            {
              
                Debug.Log($"Loading level letterBucket json from JSON");


               
                Completed = true;
                 letterBucket = Newtonsoft.Json.JsonConvert.DeserializeObject<LetterBucket>(data);
                if (letterBucket == null)
                {
                    Debug.LogError("Failed to deserialize JSON letterBucket data. >>>>>>>");
                   
                }
            }
           
        });

        while(Completed == false)
        {
            yield return new WaitForSeconds(3.0f);
        }

        Debug.Log("=== TESTING LETTER GENERATION ===");

        for (int i = 0; i < 20; i++)
        {
            string letter = WordLetterGenerationSystem.Instance.GenerateLetterV2(0, BagType.StandardLetterBag);
            Debug.Log($"Draw {i + 1}: {letter}");
        }

        // Print stats
      //  WordLetterGenerationSystem.Instance.PrintBoardStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
