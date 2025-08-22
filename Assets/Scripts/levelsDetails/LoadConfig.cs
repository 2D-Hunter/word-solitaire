using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;

public class LoadConfig : MonoBehaviour
{
    // Start is called before the first frame update
    public static LoadConfig instance;
    public static LoadConfig Instance { get { return instance; } }
    public LevelRamp loadedLevelRampData = null;
    public LetterBucket letterBucket = null;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (loadedLevelRampData == null)
        {
            StartCoroutine(LoadLevelDataFromJson());
        }
    }

    // Update is called once per frame
    IEnumerator LoadLevelDataFromJson()
    {
        // Adjust path if your JSONs are in a different Resources subfolder (e.g., "Levels/")

        string fullPathRamp = "https://2dhunter.s3.us-west-2.amazonaws.com/word-solitaire-go/fb/config/level_ramp.json";
        bool RequestCompteted = false;
        bool isError = false;
        WordServiceContainer.NetworkService.GetGameData(fullPathRamp, (issucess, data) =>
        {
            if (issucess)
            {
                RequestCompteted = true;
                isError = false;
                Debug.Log($"Loading level Ramp json from JSON");
                loadedLevelRampData = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelRamp>(data);

                Debug.Log($"Loading level from JSON: {loadedLevelRampData.Id}");
                Debug.Log($"Loading level from JSON: {loadedLevelRampData.Levels.Count}");
                if (loadedLevelRampData == null)
                {
                    Debug.LogError("Failed to deserialize JSON level data. >>>>>>>");
                    isError = true;
                }
            }
            else
            {
                RequestCompteted = false;
                isError = true;
            }
        });

        while (!RequestCompteted)
        {
            yield return new WaitForSeconds(1.0f);
        }
        yield return new WaitForSeconds(0.5f);
        if (isError)
        {
            Debug.LogError($"JSON level Ramp file not found at: /{fullPathRamp}");
            yield break;

        }
        yield return new WaitForSeconds(0.5f);
        RequestCompteted = false;
        isError = false;

        WordServiceContainer.NetworkService.GetGameData("https://2dhunter.s3.us-west-2.amazonaws.com/word-solitaire-go/fb/config/letterbucket.json", (issucess, data) =>
        {
            if (issucess)
            {
                RequestCompteted = true;
                isError = false;
                Debug.Log($"Loading level letterBucket json from JSON");
                letterBucket = Newtonsoft.Json.JsonConvert.DeserializeObject<LetterBucket>(data);
                if (letterBucket == null)
                {
                    Debug.LogError("Failed to deserialize JSON letterBucket data. >>>>>>>");
                    isError = true;
                }
            }
            else
            {
                RequestCompteted = false;
                isError = true;
            }
        });

        while (!RequestCompteted)
        {
            yield return new WaitForSeconds(1.0f);
        }
        if (isError)
        {
            Debug.LogError($"JSON level letterBucket file not found at: /{fullPathRamp}");
            yield break;

        }

    }
}
