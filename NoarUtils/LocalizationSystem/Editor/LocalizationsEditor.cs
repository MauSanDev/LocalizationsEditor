using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;

public partial class LocalizationsEditor : EditorWindow
{
    #region Paths & Files
    private static string WRAPPER_PATH = "Assets/Script/Utils/LocalizationSystem/Editor/";
    private static string WRAPPER_FILE_NAME = "LocalizationsWrapper.asset";
    private static string ISOLATED_LANGS_DIR = "Assets/Resources/LocalizationSystem/";
    private static string ISOLATED_LANGS_FILE_NAME = "Localizations_{0}.asset";
    #endregion
    
    #region Variables

    // Selected Localization
    private LocalizationsEditorDataWrapper _editorDataWrapper = null;
    private int selectedLocGroupIndex = 0;
    private int selectedLocSubGroupIndex = 0;
    private int selectedLocalizationIndex = 0;
    
    //Tabs
    private Tabs currentTab = Tabs.Explorer;

    //Result Searcher
    private Dictionary<string, List<string>> usagesResults = new Dictionary<string, List<string>>();
    
    //New Elements
    private LocalizationsEditorData _placeholderLocalizationsEditorData;
    private bool createNewGroup = false;
    private bool createNewSubGroup = false;
    private int newElementSelectedGroupIndex = 0;
    private int newElementSelectedSubGroupIndex = 0;
    
    // Wrapper Structures
    private string[] groups;
    private Dictionary<string, string[]> subgroups;
    private Dictionary<string, Dictionary<string, List<LocalizationsEditorData>>> fullStructure;
    
    #endregion
    
    #region Styles
    private GUIStyle BoldStyle => new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
    private GUIStyle TitleStyle => new GUIStyle(EditorStyles.label) { fontSize = 16, fontStyle = FontStyle.Bold };
    private GUIStyle SmallStyle => new GUIStyle(EditorStyles.label) { fontSize = 10};
    #endregion

    private LocalizationsEditorData SelectedLocalizationsEditor => _editorDataWrapper.AllLocalizationsData[selectedLocalizationIndex];

    private static List<string> PathsToIgnore => new List<string>
    {
        Application.dataPath.Replace("/Assets", "/") + ISOLATED_LANGS_DIR,
        Application.dataPath.Replace("/Assets", "/") + WRAPPER_PATH
    };

    private bool HasCreationExceptions
    {
        get
        {
            foreach(KeyValuePair<string, bool> exception in creationExceptionsDic)
            {
                if(exception.Value)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private Dictionary<string, bool> creationExceptionsDic = new Dictionary<string, bool>()
    {
        {LocalizationsEditorTexts.ERROR_REPEATED_KEY, false},
        {LocalizationsEditorTexts.ERROR_EMPTY_KEY, false},
        {LocalizationsEditorTexts.ERROR_KEY_HAS_WHITESPACES, false},
        {LocalizationsEditorTexts.ERROR_GROUP_NOT_DEFINED, false},
        {LocalizationsEditorTexts.ERROR_SUBGROUP_NOT_DEFINED, false},
        {LocalizationsEditorTexts.ERROR_PRIMARY_LANGUAGE_EMPTY, false},
    };

    private enum Tabs
    {
        InitSetup,
        Languages,
        Create,
        Explorer,
        ImportExport,
        Analyze
    }

    [MenuItem("Noar Utils/Localizations Editor")]
    public static void ShowLocalizationsEditor()
    {
        EditorWindow window = GetWindow<LocalizationsEditor>(LocalizationsEditorTexts.WINDOW_TITLE);
        window.minSize = new Vector2(1000, 500);
        window.Show();
    }

    private void OnEnable()
    {
        _editorDataWrapper = LoadLocalizationsWrapper();

        bool wrapperExists = _editorDataWrapper != null;
        bool hasRegistries = _editorDataWrapper.AllLocalizationsData.Count > 0;

        Tabs initialTab = wrapperExists ? hasRegistries ? Tabs.Explorer : Tabs.Create : Tabs.InitSetup;
        SetTab(initialTab);
            
        RefreshStructures();
    }

    private void OnGUI()
    {
        DrawToolbar();

        if(currentTab == Tabs.InitSetup)
        {
            DrawInitialSetupTab();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        DrawSidebar();

        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(position.width * 0.7f), GUILayout.Height(position.height));

        EditorGUI.indentLevel++;
        EditorGUILayout.Space();

        switch (currentTab)
        {
            case Tabs.Languages:
                DrawLanguages();
                break;
            case Tabs.Explorer:
                DrawExplorer();
                break;
            case Tabs.Create:
                DrawCreationTab();
                break;
            case Tabs.ImportExport:
                DrawImportExportTab();
                break;
            case Tabs.Analyze:
                DrawAnalyzeTab();
                break;
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
    

    private void SetTab(Tabs newTab)
    {
        switch (newTab)
        {
            case Tabs.Create:
                ClearNewValues();
                _placeholderLocalizationsEditorData = new LocalizationsEditorData(_editorDataWrapper.AvailableLanguages);
                break;
            case Tabs.Languages:
                chosenDefLang = _editorDataWrapper.PrimaryLanguage;
                chosenSysLangs = new List<SystemLanguage>(_editorDataWrapper.AvailableLanguages);
                _placeholderLocalizationsEditorData = new LocalizationsEditorData(null, null, null, null);
                currentTab = Tabs.Languages;
                break;
            case Tabs.ImportExport:
                importMethod = ImportMethod.Unselected;
                break;
        }
        currentTab = newTab;
    }
    #region Ping

    private void TryPingBuiltAssets()
    {
        if(!Directory.Exists(ISOLATED_LANGS_DIR) || Directory.GetFiles(ISOLATED_LANGS_DIR).Length == 0)
        {
            EditorUtility.DisplayDialog(LocalizationsEditorTexts.TRYPING_NO_ASSETS, LocalizationsEditorTexts.TRYPING_NO_BUILT_ASSETS_YET, LocalizationsEditorTexts.CONTINUE);
            return;
        }

        string[] files = Directory.GetFiles(ISOLATED_LANGS_DIR);
        PingObject(AssetDatabase.LoadAssetAtPath<LocalizationsData>(files[0]));
    }

    private void PingObject(Object toPing)
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = toPing;
        EditorGUIUtility.PingObject(toPing);
    }

    #endregion
    
    #region Deletion
    private void TryDeleteRegistry(LocalizationsEditorData toDelete)
    {
        int answer = EditorUtility.DisplayDialogComplex(LocalizationsEditorTexts.TRYERASE_TITLE, LocalizationsEditorTexts.TRYERASE_BODY, LocalizationsEditorTexts.DELETE, LocalizationsEditorTexts.CANCEL, string.Empty);
        if(answer == 0)
        {
            DeleteRegistry(toDelete);
        }
    }

    private void DeleteRegistry(LocalizationsEditorData toDelete)
    {
        _editorDataWrapper.DeleteRegistry(toDelete.key);
        SaveAssets();
    }
    
    #endregion
    
    #region Search Usages

    private void SearchAllUsages(bool notifyAtFinish = true)
    {
        List<string> allKeys = new List<string>(_editorDataWrapper.AllKeys);
        for(int i = 0; i < allKeys.Count; i++)
        {
            float progress = (float) i / (float) allKeys.Count;
            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.SEARCHING_TITLE, LocalizationsEditorTexts.SEARCHING_BODY.Replace("{0}", progress.ToString()) , progress);
            SearchUsages(allKeys[i]);
        }
        EditorUtility.ClearProgressBar();

        if(notifyAtFinish)
        {
            EditorUtility.DisplayDialog(LocalizationsEditorTexts.SEARCHDONE_TITLE, LocalizationsEditorTexts.SEARCHDONE_BODY , LocalizationsEditorTexts.CONTINUE);
        }
    }

    private void SearchUsages(string key)
    {
        List<string> references = ReferenceSearcher.SearchReferences(Application.dataPath, key, PathsToIgnore );
        usagesResults[key] = references;
    }

    private bool TrySearchAllUsages(bool adviceWhenFinished = true)
    {
        bool answer = EditorUtility.DisplayDialog(LocalizationsEditorTexts.SEARCHALL_TITLE, LocalizationsEditorTexts.SEARCHALL_BODY, LocalizationsEditorTexts.CONTINUE, LocalizationsEditorTexts.CANCEL);

        if (answer)
        {
            SearchAllUsages(adviceWhenFinished);
        }

        return answer;
    }

    #endregion

    #region Selection/Creation

    private void SelectLocalization(LocalizationsEditorData loc)
    {
        SelectLocalization(_editorDataWrapper.AllLocalizationsData.IndexOf(loc));
    }
    
    private void SelectLocalization(int i)
    {
        SetTab(Tabs.Explorer);
        selectedLocalizationIndex = i;
        
        for (int j = 0; j < groups.Length; j++)
        {
            if (groups[j] == SelectedLocalizationsEditor.@group)
            {
                newElementSelectedGroupIndex = j;
                for (int k = 0; k < subgroups[groups[j]].Length; k++)
                {
                    if (subgroups[groups[j]][k] == SelectedLocalizationsEditor.subGroup)
                    {
                        newElementSelectedSubGroupIndex = k;
                        return;
                    }
                }
            }
        }
    }
    
    private void CreateKey()
    {
        _editorDataWrapper.AddRegistry(_placeholderLocalizationsEditorData);
        ClearNewValues();
        SaveAssets();
        RefreshStructures();
        SelectLocalization(_editorDataWrapper.AllLocalizationsData.Count - 1); //Select the currently created key
    }
    
    private void ClearNewValues()
    {
        createNewSubGroup = false;
        createNewGroup = false;
        newElementSelectedGroupIndex = 0;
        newElementSelectedSubGroupIndex = 0;

        List<string> caca = new List<string>(creationExceptionsDic.Keys);
        foreach(string key in caca)
        {
            creationExceptionsDic[key] = false;
        }
    }

    #endregion

    #region Build Assets
    private void BuildAssets()
    {
        if(_editorDataWrapper.AllLocalizationsData.Count == 0)
        {
            EditorUtility.DisplayDialog(LocalizationsEditorTexts.BUILDASSETS_NOLOCS_TITLE, LocalizationsEditorTexts.BUILDASSETS_NOLOCS_BODY, LocalizationsEditorTexts.CONTINUE);
            return;
        }

        if(!Directory.Exists(ISOLATED_LANGS_DIR))
        {
            Directory.CreateDirectory(ISOLATED_LANGS_DIR);
        }

        Dictionary<SystemLanguage, LocalizationsData> filesToCreate = new Dictionary<SystemLanguage, LocalizationsData>();

        for(int i = 0; i < _editorDataWrapper.AvailableLanguages.Count; i++)
        {
            SystemLanguage lang = _editorDataWrapper.AvailableLanguages[i];
            
            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.BUILDASSETS_BUILDING_TITLE, LocalizationsEditorTexts.BUILDASSETS_BUILDING_BODY.Replace("{0}", lang.ToString()), i / _editorDataWrapper.AvailableLanguages.Count);

            filesToCreate[lang] = CreateInstance<LocalizationsData>();
            filesToCreate[lang].ChangeLanguage(lang);
        }


        for (int i = 0; i < _editorDataWrapper.AllLocalizationsData.Count; i++)
        {
            LocalizationsEditorData loc = _editorDataWrapper.AllLocalizationsData[i];

            string barBodyText = LocalizationsEditorTexts.BUILDASSETS_PARSING_BODY.Replace("{0}", loc.key)
                .Replace("{1}", i.ToString()).Replace("{2}", _editorDataWrapper.AllLocalizationsData.Count.ToString());
            
            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.BUILDASSETS_BUILDING_TITLE, barBodyText, i / _editorDataWrapper.AllLocalizationsData.Count);

            for (int j = 0; j < loc.languages.Count; j++)
            {
                filesToCreate[loc.languages[j]].AddLocalization(loc.key, loc.translations[j]);
            }
        }

        foreach(LocalizationsData file in filesToCreate.Values)
        {
            string assetPath = Path.Combine(ISOLATED_LANGS_DIR, ISOLATED_LANGS_FILE_NAME.Replace("{0}", file.Language.ToString()));

            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.BUILDASSETS_BUILDING_TITLE, LocalizationsEditorTexts.BUILDASSETS_CREATINGASSET_BODY.Replace("{0}", file.Language.ToString()), 0.99f);

            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
            }
            AssetDatabase.CreateAsset(file, assetPath);
        }

        EditorUtility.ClearProgressBar();

        EditorUtility.DisplayDialog(LocalizationsEditorTexts.DONE,LocalizationsEditorTexts.BUILDASSETS_ASSETSCREATED_BODY , LocalizationsEditorTexts.CONTINUE);
    }
    #endregion
    
    #region Load/Save
    private void RefreshStructures()
    {
        if (_editorDataWrapper == null)
        {
            return;
        }
        
        groups = _editorDataWrapper.GroupNames;
        subgroups = _editorDataWrapper.GroupsStructure;
        fullStructure = _editorDataWrapper.FullStructure;
    }

    public static LocalizationsEditorDataWrapper LoadLocalizationsWrapper()
    {
        return AssetDatabase.LoadAssetAtPath<LocalizationsEditorDataWrapper>(Path.Combine(WRAPPER_PATH, WRAPPER_FILE_NAME));
    }
    
    private void CreateWrapper()
    {
        if(!Directory.Exists(WRAPPER_PATH))
        {
            Directory.CreateDirectory(WRAPPER_PATH);
        }

        _editorDataWrapper = CreateInstance<LocalizationsEditorDataWrapper>();
        LocalizationSystem.SetDefaultLanguage(primaryLanguage);
        _editorDataWrapper.OverrideLanguages(primaryLanguage, chosenSysLangs);

        AssetDatabase.CreateAsset(_editorDataWrapper, Path.Combine(WRAPPER_PATH, WRAPPER_FILE_NAME));
        AssetDatabase.SaveAssets();
    }

    private void SaveAssets()
    {
        EditorUtility.SetDirty(_editorDataWrapper);
        AssetDatabase.SaveAssets();
    }
    #endregion
}
