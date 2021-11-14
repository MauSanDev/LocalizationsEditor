using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationSystem
{
    private const string DEFAULT_LANG_PREF = "LocalizationSystem_DefaultLanguageString";
    private const string CURRENT_LANG_PREF = "LocalizationSystem_CurrentLanguageString";

    private static Dictionary<string, string> localizationData = null;
    
    private static void SetLocalizations(LocalizationsData database)
    {
        localizationData = database.Localizations;
    }

    [RuntimeInitializeOnLoadMethod]
    public static void LoadLocalizations()
    {
        LocalizationsData localizationsDataFile = Resources.Load<LocalizationsData>($"LocalizationSystem/Localizations_{SettledLanguage}");

        if (localizationsDataFile == null)
        {
            Debug.LogError($"LocalizationSystem :: Cannot load Localizations for {SettledLanguage}.");
            return;
        }
        
        Debug.Log($"LocalizationSystem :: Loading Localizations for Language {SettledLanguage}");
        SetLocalizations(localizationsDataFile);
        
    }
    
    public static void SetDefaultLanguage(SystemLanguage language)
    {
        Debug.Log($"Settled default lang to {language}");
        PlayerPrefs.SetString(DEFAULT_LANG_PREF, language.ToString());
    }

    public static void SetDeviceLanguage(SystemLanguage language)
    {
        Debug.Log($"Settled Device lang to {language}");
        PlayerPrefs.SetString(CURRENT_LANG_PREF, language.ToString());
        if (Application.isPlaying && SettledLanguage != language)
        {
            LoadLocalizations();
        }
    }
    
    public static SystemLanguage SettledLanguage
    {
        get
        {
            if (PlayerPrefs.HasKey(CURRENT_LANG_PREF) && Enum.TryParse(PlayerPrefs.GetString(CURRENT_LANG_PREF), out SystemLanguage currentLanguage) )
            {
                return currentLanguage;
            }

            return DefaultLanguage;
        }
    }
    
    public static SystemLanguage DefaultLanguage
    {
        get
        {
            if (PlayerPrefs.HasKey(DEFAULT_LANG_PREF) && Enum.TryParse(PlayerPrefs.GetString(DEFAULT_LANG_PREF), out SystemLanguage currentLanguage) )
            {
                return currentLanguage;
            }

            return Application.systemLanguage;
        }
    }

    
    public static string Localize(string key)
    {
        if(localizationData == null)
        {
            Debug.LogError($"LocalizationSystem :: The Localization Database is not Inited. The key {key} can't be localized.");
            return key;
        }

        if(localizationData.ContainsKey(key) && !string.IsNullOrEmpty(localizationData[key]))
        {
            return localizationData[key];
        }

        return key;
    }
}
