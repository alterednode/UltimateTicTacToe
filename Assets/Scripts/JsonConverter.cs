using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class JsonConverter
{
    // Converts a List<KeyValuePair<string, string>> to a JSON string
    public static string ListToJson(List<KeyValuePair<string, string>> list)
    {
        Debug.Log($"ListToJson: Input list count: {list.Count}");

        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendFormat("\"{0}\":\"{1}\"", list[i].Key, list[i].Value);
            if (i < list.Count - 1)
                sb.Append(",");
        }
        sb.Append("}");

        string result = sb.ToString();
        Debug.Log($"ListToJson: Output JSON: {result}");
        return result;
    }

    // Converts a JSON string to a List<KeyValuePair<string, string>>
    public static List<KeyValuePair<string, string>> JsonToList(string jsonString)
    {
        Debug.Log($"JsonToList: Input JSON: {jsonString}");

        var list = new List<KeyValuePair<string, string>>();

        try
        {
            var wrapper = JsonUtility.FromJson<Wrapper>($"{{\"Items\":{jsonString}}}");
            if (wrapper == null || wrapper.Items == null)
            {
                throw new Exception("Parsed wrapper or Items is null");
            }
            Debug.Log($"JsonToList: Wrapper parsed successfully. Item count: {wrapper.Items.Count}");

            foreach (var kvp in wrapper.Items)
            {
                list.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value));
                Debug.Log($"JsonToList: Added pair - Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonToList: Error parsing JSON: {e.Message}");
            Debug.LogError($"JsonToList: Stack trace: {e.StackTrace}");
        }

        Debug.Log($"JsonToList: Output list count: {list.Count}");
        return list;
    }

    // Helper class to deserialize JSON

    [Serializable]
    private class Wrapper
    {
        public Dictionary<string, string> Items;
    }
}