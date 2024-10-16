using System.Collections.Generic;
using UnityEngine;

public static class JsonDictionaryConverter
{
    // Converts a Dictionary<string, string> to a JSON string
    public static string DictionaryToJson(Dictionary<string, string> dictionary)
    {
        return JsonUtility.ToJson(new Serialization<string, string>(dictionary));
    }

    // Converts a JSON string to a Dictionary<string, string>
    public static Dictionary<string, string> JsonToDictionary(string jsonString)
    {
        return JsonUtility.FromJson<Serialization<string, string>>(jsonString).ToDictionary();
    }

    [System.Serializable]
    private class Serialization<TKey, TValue>
    {
        public List<TKey> keys;
        public List<TValue> values;

        public Serialization(Dictionary<TKey, TValue> dictionary)
        {
            keys = new List<TKey>(dictionary.Keys);
            values = new List<TValue>(dictionary.Values);
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keys.Count; i++)
            {
                dictionary[keys[i]] = values[i];
            }
            return dictionary;
        }
    }
}