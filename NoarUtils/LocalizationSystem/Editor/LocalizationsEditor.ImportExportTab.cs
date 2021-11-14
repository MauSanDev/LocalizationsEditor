using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public partial class LocalizationsEditor
{
    private const string KEY_CSV_ROW = "Key";
    private const string GROUP_CSV_ROW = "Group";
    private const string SUBGROUP_CSV_ROW = "SubGroup";

    private ImportMethod importMethod = ImportMethod.Unselected;
    

    private enum ImportMethod
    {
        Unselected,
        OverrideAll, // Clean all the existing localizations and import all the CSV ones
        OverrideMatches, // Override only localizations that already exists but don't delete the ones that are not on the new file (also import new ones)
        ImportOnlyNew // Only import the keys that aren't on the project and don't touch the existing ones.
    }
    
    private void DrawImportExportTab()
    {
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_TAB_TITLE, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_TAB_DESC, SmallStyle);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawCSVBaseOption();
        DrawExportOption();

        DrawImportOption();

    }

    #region Export Options
    private void DrawExportOption()
    {
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_EXPORT, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_EXPORT_DESC, SmallStyle);

        if (GUILayout.Button(LocalizationsEditorTexts.IE_EXPORT_BUTTON))
        {
            GenerateCSVFile(GetCSVSavePath(PlayerSettings.productName.Replace(" ", "") + "_LocalizationData"), CSVLocalizationsData);
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawCSVBaseOption()
    {
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_GENERATE_BASE, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_GENERATE_BASE_DESC, SmallStyle);

        if (GUILayout.Button(LocalizationsEditorTexts.IE_GENERATE_BASE_BUTTON))
        {
            GenerateCSVFile(GetCSVSavePath(PlayerSettings.productName.Replace(" ", "") + "_LocalizationData_Base"), CSVFirstLine);
        }
        EditorGUILayout.EndVertical();
    }

    private string GetCSVSavePath(string defaultName)
    {
        return EditorUtility.SaveFilePanel(LocalizationsEditorTexts.IE_SAVE_CSV_PANEL_TITLE, "", defaultName, "csv");
    }

    private void GenerateCSVFile(string filePath, string content)
    {
        if (CheckForFilePath(filePath))
        {
            CreateTextFile(filePath, content);
            OnCSVFileCreated(filePath);
        }
    }

    private bool CheckForFilePath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        return true;
    }

    private void OnCSVFileCreated(string filePath)
    {
        int result = EditorUtility.DisplayDialogComplex(LocalizationsEditorTexts.IE_FILE_CREATED, LocalizationsEditorTexts.IE_FILE_CREATED_BODY + filePath, LocalizationsEditorTexts.CONTINUE, LocalizationsEditorTexts.IE_VIEW_FILE, "");
        if (result == 1)
        {
            EditorUtility.RevealInFinder(filePath);
        }
    }
    private void CreateTextFile(string path, string content)
    {
        StreamWriter sw = File.CreateText(path);
        sw.Write(content);
        sw.Close();
    }

    private string CSVLocalizationsData
    {
        get
        {
            string toReturn = CSVFirstLine;

            for (int i = 0; i < _editorDataWrapper.AllLocalizationsData.Count; i++)
            {
                LocalizationsEditorData currentKey = _editorDataWrapper.AllLocalizationsData[i];
                toReturn += currentKey.CSVFormat;
            }

            return toReturn;
        }
    }

    private string CSVFirstLine
    {
        get
        {
            string firstLine = $"{KEY_CSV_ROW};{GROUP_CSV_ROW};{SUBGROUP_CSV_ROW};";
            foreach (SystemLanguage lang in _editorDataWrapper.AvailableLanguages)
            {
                firstLine += lang + ";";
            }
            firstLine += "\n";
            return firstLine;
        }
    }

    #endregion

    #region Import options
    private void DrawImportOption()
    {
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_IMPORT, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_IMPORT_DESC, SmallStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.IE_IMPORT_NOTE, SmallStyle);

        EditorGUILayout.BeginHorizontal();
        importMethod = (ImportMethod) EditorGUILayout.EnumPopup(LocalizationsEditorTexts.IE_IMPORT_METHOD, importMethod);

        if (GUILayout.Button(LocalizationsEditorTexts.IE_IMPORT_BUTTON))
        {
            if(importMethod == ImportMethod.Unselected)
            {
                EditorUtility.DisplayDialog(LocalizationsEditorTexts.IE_IMPORT_SELECT_METHOD_TITLE, LocalizationsEditorTexts.IE_IMPORT_SELECT_METHOD_BODY, LocalizationsEditorTexts.CONTINUE);
            }
            else
            {
                ImportCSVFile(EditorUtility.OpenFilePanel(LocalizationsEditorTexts.IE_IMPORT_SEARCH_FILE_TITLE, "", "csv"), importMethod);
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void ImportCSVFile(string filePath, ImportMethod importMethod)
    {
        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }

        if(importMethod == ImportMethod.OverrideAll)
        {
            _editorDataWrapper.AllLocalizationsData.Clear();
        }

        EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.IE_IMPORTING_CSV_HEADER,LocalizationsEditorTexts.IE_IMPORTING_CSV_READING , 0f);

        string[] lines = File.ReadAllLines(filePath);

        List<Dictionary<string, string>> rawKeysData = new List<Dictionary<string, string>>();

        string[] firstRowData = lines[0].Split(';');

        for(int i = 1; i < lines.Length; i++)
        {
            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.IE_IMPORTING_CSV_HEADER,LocalizationsEditorTexts.IE_IMPORTING_CSV_PARSING , (float)i / lines.Length);

            Dictionary<string, string> rowData = new Dictionary<string, string>();
            string[] cells = lines[i].Split(';');

            for(int j = 0; j < firstRowData.Length; j++)
            {
                if(string.IsNullOrEmpty(firstRowData[j]))
                {
                    continue;
                }

                rowData[firstRowData[j]] = cells[j];
            }
            rawKeysData.Add(rowData);
        }

        for(int i = 0; i < rawKeysData.Count; i++)
        {
            Dictionary<string, string> rawKey = rawKeysData[i];

            if (!rawKey.ContainsKey(KEY_CSV_ROW))
            {
                Debug.LogError(LocalizationsEditorTexts.IE_IMPORTING_CSV_EMPTY_KEY);
                continue;
            }

            string key = rawKey[KEY_CSV_ROW];
            string group = rawKey.ContainsKey(GROUP_CSV_ROW) ? rawKey[GROUP_CSV_ROW] : string.Empty;
            string subGroup = rawKey.ContainsKey(SUBGROUP_CSV_ROW) ? rawKey[SUBGROUP_CSV_ROW] : string.Empty;


            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.IE_IMPORTING_CSV_HEADER,LocalizationsEditorTexts.IE_IMPORTING_CSV_IMPORTING_KEY , (float)i / lines.Length);

            if (importMethod == ImportMethod.OverrideMatches)
            {
                if(_editorDataWrapper.ContainsKey(key))
                {
                    _editorDataWrapper.DeleteRegistry(key);
                }    
                else
                {
                    continue;
                }
            }

            if(importMethod == ImportMethod.ImportOnlyNew)
            {
                if(_editorDataWrapper.ContainsKey(key))
                {
                    continue;
                }
            }
            
            Dictionary<SystemLanguage, string> addedLanguages = new Dictionary<SystemLanguage, string>();

            foreach(KeyValuePair<string, string> cell in rawKey)
            {
                if(Enum.TryParse(cell.Key, out SystemLanguage lang))
                {
                    addedLanguages.Add(lang, cell.Value);
                }
            }

            List<string> translations = new List<string>();
            foreach(SystemLanguage lang in _editorDataWrapper.AvailableLanguages)
            {
                if(addedLanguages.ContainsKey(lang))
                {
                    translations.Add(addedLanguages[lang]);
                }
            }

            _editorDataWrapper.AddRegistry(key, string.Empty, group, subGroup, translations);
        }

        RefreshStructures();
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog(LocalizationsEditorTexts.IE_IMPORTING_SUCCESS_TITLE, LocalizationsEditorTexts.IE_IMPORTING_SUCCESS_TITLE, LocalizationsEditorTexts.CONTINUE);
    }
    #endregion
}
