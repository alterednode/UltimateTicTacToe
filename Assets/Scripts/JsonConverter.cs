using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;

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
        

        var list = new List<KeyValuePair<string, string>>();

        try
        {
            // Use regex to find matches for key-value pairs
            var matches = Regex.Matches(jsonString, "\"(.*?)\":\"(.*?)\"");
            if (matches.Count == 0)
            {
                Debug.LogError("No matches found in the JSON string");
            }

            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    list.Add(new KeyValuePair<string, string>(key, value));
                    Debug.Log($"JsonToList: Pair - Key: {key}, Value: {value}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonToList: Error parsing JSON: {e.Message}");
            Debug.LogError($"JsonToList: Stack trace: {e.StackTrace}");
        }

       // Debug.Log($"JsonToList: Output list count: {list.Count}");
        return list;
    }
}