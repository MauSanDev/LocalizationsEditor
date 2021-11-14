using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalizationsEditorData
{
    [SerializeField] public string key = "";
    [SerializeField] public string description = "";
    [SerializeField] public string group = "";
    [SerializeField] public string subGroup = "";
    [SerializeField] public List<SystemLanguage> languages = new List<SystemLanguage>();
    [SerializeField] public List<string> translations = new List<string>();

    public LocalizationsEditorData(List<SystemLanguage> languages)
    {
        this.languages = languages;
        for (int i = 0; i < languages.Count; i++)
        {
            translations.Add("");
        }
    }

    public LocalizationsEditorData(string key, string description, List<SystemLanguage> languages, List<string> translations, string group = null, string subGroup = null)
    {
        this.key = key;
        this.description = description;
        this.languages = languages;
        this.translations = translations;
        this.group = group;
        this.subGroup = subGroup;
    }

    public Dictionary<SystemLanguage, string> Localizations
    {
        get
        {
            Dictionary<SystemLanguage, string> toReturn = new Dictionary<SystemLanguage, string>();
            for(int i = 0; i < languages.Count; i++)
            {
                toReturn.Add(languages[i], translations[i]);
            }
            return toReturn;
        }
    }

    public bool AreAllLanguagesTranslated
    {
        get
        {
            foreach(string translation in Localizations.Values)
            {
                if(string.IsNullOrEmpty(translation))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public string CSVFormat => $"{key};{group};{subGroup};" + string.Join(";", translations) + "\n";

    public string Group => group;
    public string SubGroup => subGroup;

    public void AddLanguage(SystemLanguage newLang)
    {
        if(!languages.Contains(newLang))
        {
            languages.Add(newLang);
            translations.Add(string.Empty);
        }
    }
    
    public void OverrideLanguages(List<SystemLanguage> langs)
    {
        List<string> orderedNewValues= new List<string>();

        for(int i = 0; i < langs.Count; i++)
        {
            if(languages.Contains(langs[i]))
            {
                orderedNewValues.Add(translations[languages.IndexOf(langs[i])]);
            }
            else
            {
                orderedNewValues.Add(string.Empty);
            }
        }

        languages = langs;
        translations = orderedNewValues;
    }

    public void RemoveLanguage(SystemLanguage lang)
    {
        if(languages.Contains(lang))
        {
            int index = languages.IndexOf(lang);
            languages.RemoveAt(index);
            translations.RemoveAt(index);
        }
    }
}
