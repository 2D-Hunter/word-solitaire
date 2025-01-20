
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Text; // Required for StringBuilder

public class WordnikDefinition : MonoBehaviour
{
    public string apiUrl = "https://en.wiktionary.org/w/api.php";
    public static WordnikDefinition instance;

    private void Start()
    {
        instance = this;
    }

    public void FetchDefinition(string word)
    {
        StartCoroutine(FetchDefinitionCoroutine(word));
    }

    private IEnumerator FetchDefinitionCoroutine(string word)
    {
        if (Dictionary.instance.loading != null)
            Dictionary.instance.loading.SetActive(true);

        string requestUrl = $"{apiUrl}?action=query&prop=extracts&titles={word}&format=json&explaintext=true";
        Debug.Log("URL: " + requestUrl);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl))
        {
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching definition: " + webRequest.error);
                Dictionary.instance.definition.text = "Error fetching definition. Please try again.";
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("API Response: " + jsonResponse);

                string definition = ParseDefinition(jsonResponse);

                Dictionary.instance.definition.text = string.IsNullOrEmpty(definition) ? "No definition found." : definition;
            }
        }

        if (Dictionary.instance.loading != null)
            Dictionary.instance.loading.SetActive(false);
    }

    private string ExtractDefinition(string fullExtract)
    {
        const string nounSectionStart = "=== Noun ===";
        const string sectionEndMarker = "===";

        string lowerFullExtract = fullExtract.ToLower();
        string lowerNounSectionStart = nounSectionStart.ToLower();
        string lowerSectionEndMarker = sectionEndMarker.ToLower();

        // Look for the "Noun" section
        int nounStartIndex = lowerFullExtract.IndexOf(lowerNounSectionStart);
        if (nounStartIndex == -1) return "No definition found.";

        // Extract everything after "=== Noun ==="
        int definitionStartIndex = nounStartIndex + nounSectionStart.Length;
        int sectionEndIndex = lowerFullExtract.IndexOf(lowerSectionEndMarker, definitionStartIndex);

        // If no other section is found, take the rest of the text
        if (sectionEndIndex == -1)
        {
            sectionEndIndex = fullExtract.Length;
        }

        // Get the text for the Noun section
        string nounDefinition = fullExtract.Substring(definitionStartIndex, sectionEndIndex - definitionStartIndex).Trim();

        // Remove text within parentheses
        nounDefinition = System.Text.RegularExpressions.Regex.Replace(nounDefinition, @"\([^)]*\)", "").Trim();

        // Split the definitions into lines
        string[] lines = nounDefinition.Split(new char[] { '\n' });

        // Create formatted output
        StringBuilder formattedDefinition = new StringBuilder();
        int counter = 1;

        foreach (var line in lines)
        {
            // Skip empty lines, irrelevant headers, and lines with "Synonyms" or similar non-definitions
            if (string.IsNullOrWhiteSpace(line) ||
                line.StartsWith("====") ||
                line.Trim().StartsWith("beer") ||
                line.ToLower().Contains("synonyms"))
            {
                continue;
            }

            // Only process lines that are likely valid definitions
            if (line.Length > 5) // Filter out lines that are too short
            {
                // Format each line with a number and part of speech
                formattedDefinition.AppendLine($"{counter}. (noun) {line.Trim()}");
                if (counter < 2)
                {
                    formattedDefinition.AppendLine();
                }
                counter++;

                if (counter > 2) break;
            }
        }

        return formattedDefinition.Length > 0 ? formattedDefinition.ToString().Trim() : "No definition found.";
    }

    private string ParseDefinition(string jsonResponse)
    {
        try
        {
            var response = JObject.Parse(jsonResponse);

            // Access the "pages" section
            var pages = response["query"]?["pages"];
            if (pages != null)
            {
                foreach (var page in pages)
                {
                    var extract = page.First?["extract"]?.ToString();
                    if (!string.IsNullOrEmpty(extract))
                    {
                        // Extract and return the relevant part of the definition
                        return ExtractDefinition(extract);
                    }
                }
            }
            return "No definition found for this word.";
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing JSON: " + e.Message);
            return "Error parsing the definition.";
        }
    }

    [System.Serializable]
    private class MediaWikiResponse
    {
        public Query query;
    }

    [System.Serializable]
    private class Query
    {
        public Dictionary<string, Page> pages;
    }

    [System.Serializable]
    private class Page
    {
        public string extract;
    }
}