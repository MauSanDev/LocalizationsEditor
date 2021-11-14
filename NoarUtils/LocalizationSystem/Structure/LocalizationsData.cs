using System.Collections.Generic;
using UnityEngine;

public class LocalizationsData : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private SystemLanguage language = SystemLanguage.English;
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<string> values = new List<string>();
    
    private Dictionary<string, string> localizations = new Dictionary<string, string>();
    
    public SystemLanguage Language => language;
    public Dictionary<string, string> Localizations => localizations;

    public void ChangeLanguage(SystemLanguage lang)
    {
        language = lang;
    }

    public void AddLocalization(string key, string value)
    {
        localizations.Add(key, value);
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        
        foreach (string key in localizations.Keys)
        {
            keys.Add(key);
            values.Add(localizations[key]);
        }
    }

    public void OnAfterDeserialize()
    {
        localizations.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            localizations.Add(keys[i], values[i]);
        }
    }
}
