using System.Collections.Generic;
using UnityEngine;

public static class JsonConverter
{
    // Converts a List<KeyValuePair<string, string>> to a JSON string
    public static string ListToJson(List<KeyValuePair<string, string>> list)
    {
        return JsonUtility.ToJson(new Serialization<string, string>(list));
    }

    // Converts a JSON string to a List<KeyValuePair<string, string>>
    public static List<KeyValuePair<string, string>> JsonToList(string jsonString)
    {
        return JsonUtility.FromJson<Serialization<string, string>>(jsonString).ToList();
    }

    [System.Serializable]
    private class Serialization<TKey, TValue>
    {
        public List<KeyValuePair<TKey, TValue>> keyValuePairs;

        public Serialization(List<KeyValuePair<TKey, TValue>> list)
        {
            keyValuePairs = new List<KeyValuePair<TKey, TValue>>(list);
        }

        public List<KeyValuePair<TKey, TValue>> ToList()
        {
            return new List<KeyValuePair<TKey, TValue>>(keyValuePairs);
        }
    }
}