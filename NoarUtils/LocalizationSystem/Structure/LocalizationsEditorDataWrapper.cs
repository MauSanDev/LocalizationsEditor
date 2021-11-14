using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalizationsEditorDataWrapper : ScriptableObject
{
    [SerializeField] private SystemLanguage primaryLanguage = SystemLanguage.English;
    [SerializeField] private List<SystemLanguage> availableLanguages = new List<SystemLanguage>();
    [SerializeField] private List<LocalizationsEditorData> localizations = new List<LocalizationsEditorData>();
    
    public SystemLanguage PrimaryLanguage => primaryLanguage;
    public List<SystemLanguage> AvailableLanguages
    {
        get
        {
            List<SystemLanguage> langs = new List<SystemLanguage>(availableLanguages);
            langs.Insert(0, primaryLanguage);
            return langs;
        }
    }
    
    public List<LocalizationsEditorData> AllLocalizationsData => localizations;

    public HashSet<string> AllKeys
    {
        get
        {
            HashSet<string> toReturn = new HashSet<string>();
            foreach(LocalizationsEditorData locData in AllLocalizationsData)
            {
                toReturn.Add(locData.key);
            }
            return toReturn;
        }
    }
    
    public string[] GroupNames 
    {
        get
        {
            HashSet<string> groupNames = new HashSet<string>();
            for(int i = 0; i < localizations.Count; i++)
            {
                if (string.IsNullOrEmpty(localizations[i].group))
                {
                    continue;
                }
                groupNames.Add(localizations[i].group);
            }
            return groupNames.ToArray();
        }
    }

    public Dictionary<string, string[]> GroupsStructure
    {
        get
        {
            Dictionary<string, HashSet<string>> hashStructure = new Dictionary<string, HashSet<string>>();
            Dictionary<string, string[]> toReturn = new Dictionary<string, string[]>();
            
            LocalizationsEditorData currentLocalizationsEditor;
            
            for (int i = 0; i < localizations.Count; i++)
            {
                currentLocalizationsEditor = localizations[i];
                if (!string.IsNullOrEmpty(currentLocalizationsEditor.group) && !hashStructure.ContainsKey(currentLocalizationsEditor.group))
                {
                    hashStructure.Add(currentLocalizationsEditor.group, new HashSet<string>());
                }

                if (!string.IsNullOrEmpty(currentLocalizationsEditor.subGroup))
                {
                    hashStructure[currentLocalizationsEditor.group].Add(currentLocalizationsEditor.subGroup);
                }
            }

            foreach (string key in hashStructure.Keys)
            {
                toReturn.Add(key, hashStructure[key].ToArray());
            }

            return toReturn;
        }
    }

    public Dictionary<string, Dictionary<string, List<LocalizationsEditorData>>> FullStructure
    {
        get
        {
            Dictionary<string, Dictionary<string, List<LocalizationsEditorData>>> toReturn = new Dictionary<string, Dictionary<string, List<LocalizationsEditorData>>>();
            
            LocalizationsEditorData currentLocalizationsEditor;
            
            for (int i = 0; i < localizations.Count; i++)
            {
                currentLocalizationsEditor = localizations[i];
                if (!string.IsNullOrEmpty(currentLocalizationsEditor.group) && !toReturn.ContainsKey(currentLocalizationsEditor.group))
                {
                    toReturn.Add(currentLocalizationsEditor.group, new Dictionary<string, List<LocalizationsEditorData>>());
                }
                
                if (!string.IsNullOrEmpty(currentLocalizationsEditor.subGroup) && !toReturn[currentLocalizationsEditor.group].ContainsKey(currentLocalizationsEditor.subGroup))
                {
                    toReturn[currentLocalizationsEditor.group].Add(currentLocalizationsEditor.subGroup, new List<LocalizationsEditorData>());
                }

                if (!string.IsNullOrEmpty(currentLocalizationsEditor.subGroup))
                {
                    toReturn[currentLocalizationsEditor.group][currentLocalizationsEditor.subGroup].Add(currentLocalizationsEditor);
                }
            }

            return toReturn;
        }
    }

    public bool ContainsKey(string key) => AllLocalizationsData.Find(x => x.key == key) != null;
    public bool IsRepeated(string key) => AllLocalizationsData.FindAll(x => x.key == key).Count > 1;
    public void DeleteRegistry(string key) => localizations.RemoveAll(x => x.key == key);

    public void AddRegistry(string key, string desc, string group, string subgroup, List<string> values)
    {
        LocalizationsEditorData newLoc = new LocalizationsEditorData(key, desc, AvailableLanguages, values, group, subgroup);
        AddRegistry(newLoc);
    }
    public void AddRegistry(LocalizationsEditorData newLocalizationsEditor)
    {
        localizations.Add(newLocalizationsEditor);
    }

    public bool ContainsLanguage(SystemLanguage lang) => availableLanguages.Contains(lang) || lang != primaryLanguage;

    public void AddLanguage(SystemLanguage lang)
    {
        if(!ContainsLanguage(lang))
        {
            availableLanguages.Add(lang);
            foreach (LocalizationsEditorData subgroup in localizations)
            {
                subgroup.AddLanguage(lang);
            }
        }
    }

    public void OverrideLanguages(SystemLanguage primary, List<SystemLanguage> languages)
    {
        primaryLanguage = primary;
        List<SystemLanguage> availableLanguagesCopy = new List<SystemLanguage>(languages);

        availableLanguagesCopy.Remove(primary); // in case it is included.
        availableLanguagesCopy.Sort((x, y) => x.CompareTo(y));
        availableLanguages = new List<SystemLanguage>(availableLanguagesCopy);
        availableLanguagesCopy.Insert(0, primary);

        foreach (LocalizationsEditorData localization in localizations)
        {
            localization.OverrideLanguages(availableLanguagesCopy);
        }
    }
    
    public void RemoveLanguage(SystemLanguage lang)
    {
        if (availableLanguages.Contains(lang))
        {
            availableLanguages.Remove(lang);
            foreach (LocalizationsEditorData subgroup in localizations)
            {
                subgroup.RemoveLanguage(lang);
            }
        }
    }

    public string Localize(string key, SystemLanguage lang)
    {
        if(!AvailableLanguages.Contains(lang))
        {
            return key;
        }

        LocalizationsEditorData locData = AllLocalizationsData.Find(x => x.key == key);
        if(locData == null || string.IsNullOrEmpty(locData.Localizations[lang]))
        {
            return key;
        }

        return locData.Localizations[lang];
    }

}
