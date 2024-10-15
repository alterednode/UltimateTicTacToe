using System.Collections.Generic;

[System.Serializable]
public class StringStringDictionary
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    // Optional: Constructor for convenience
    public StringStringDictionary() { }

    // Method to convert to Dictionary<string, string>
    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
        return dictionary;
    }

    // Method to populate from Dictionary<string, string>
    public void FromDictionary(Dictionary<string, string> dict)
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in dict)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }
}
