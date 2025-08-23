
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// This class forces Unity's IL2CPP/AOT compiler to preserve Newtonsoft.Json types
// so they don’t get stripped in WebGL builds.
public static class AotTypePreserver
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PreserveNewtonsoftTypes()
    {
        // Force references so IL2CPP keeps them
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

        // Force usage of JsonConvert
        string test = JsonConvert.SerializeObject("test", settings);
        string result = JsonConvert.DeserializeObject<string>(test, settings);

        // JToken & JObject preservation
        JToken token = JToken.Parse("{\"foo\":\"bar\"}");
        JObject obj = JObject.FromObject(new { foo = "bar" });

        UnityEngine.Debug.Log("AotTypePreserver initialized. Newtonsoft.Json types preserved.");
    }
}