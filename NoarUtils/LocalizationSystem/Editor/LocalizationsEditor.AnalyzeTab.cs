using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class LocalizationsEditor
{
    private Vector2 analyzeTabScroll = Vector2.zero;
    private AnalysisMode analysisMode = AnalysisMode.All;

    private Dictionary<AnalysisMode, List<string>> analysisResults = new Dictionary<AnalysisMode, List<string>>();

    private Dictionary<AnalysisMode, string> analysisLogs = new Dictionary<AnalysisMode, string>()
    {
        { AnalysisMode.NotTranslatedKeys, LocalizationsEditorTexts.ANALYSIS_NOT_TRANSLATED_ERROR },
        { AnalysisMode.EmptyGroups, LocalizationsEditorTexts.ANALYSIS_EMPTY_GROUPS_ERROR },
        { AnalysisMode.EmptySubGroups, LocalizationsEditorTexts.ANALYSIS_EMPTY_SUBGROUPS_ERROR },
        { AnalysisMode.RepeatedElements, LocalizationsEditorTexts.ANALYSIS_REPEATED_ELEMENTS_ERROR },
        { AnalysisMode.ElementsWithoutUse, LocalizationsEditorTexts.ANALYSIS_ELEMENTS_WITHOUT_USE_ERROR }
    };

    [Flags]
    private enum AnalysisMode
    {
        NotTranslatedKeys = 1,
        EmptyGroups = 2,
        EmptySubGroups = 4,
        RepeatedElements = 8,
        ElementsWithoutUse = 16,
        All = NotTranslatedKeys | EmptyGroups | EmptySubGroups | RepeatedElements | ElementsWithoutUse
    }

    private void DrawFlagToggle(string label, AnalysisMode analysisFlag)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Toggle(label, analysisMode.HasFlag(analysisFlag), GUILayout.Width(200));
        if(EditorGUI.EndChangeCheck())
        {
            if(analysisMode.HasFlag(analysisFlag))
            {
                analysisMode -= analysisFlag;
            }
            else
            {
                analysisMode = analysisMode | analysisFlag;
            }
        }
    }

    private void DrawAnalyzeTab()
    {
        EditorGUILayout.LabelField(LocalizationsEditorTexts.ANALYSIS_TAB_NAME, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.ANALYSIS_TAB_DESCRIPTION, SmallStyle);

        EditorGUILayout.Space();

        DrawSearchCriteria();


        GUI.backgroundColor = Color.black;
        EditorGUILayout.BeginScrollView(analyzeTabScroll, "HelpBox");
        DrawResult();
        EditorGUILayout.EndScrollView();
        GUI.backgroundColor = Color.white;
    }

    private void DrawSearchCriteria()
    {
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.ANALYZING_SEARCH_CRITERIA, BoldStyle);

        EditorGUILayout.BeginHorizontal();
        DrawFlagToggle(LocalizationsEditorTexts.ANALYSIS_NOT_TRANSLATED, AnalysisMode.NotTranslatedKeys);
        DrawFlagToggle(LocalizationsEditorTexts.ANALYSIS_EMPTY_GROUPS, AnalysisMode.EmptyGroups);
        DrawFlagToggle(LocalizationsEditorTexts.ANALYSIS_EMPTY_SUBGROUPS, AnalysisMode.EmptySubGroups);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        DrawFlagToggle(LocalizationsEditorTexts.ANALYSIS_REPEATED_ELEMENTS, AnalysisMode.RepeatedElements);
        DrawFlagToggle(LocalizationsEditorTexts.ANALYSIS_ELEMENTS_WITHOUT_USE, AnalysisMode.ElementsWithoutUse);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUI.backgroundColor = Color.gray;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(LocalizationsEditorTexts.ANALYSIS_TAB_NAME))
        {
            ExecuteAnalysis();
        }
        if (GUILayout.Button(LocalizationsEditorTexts.CLEAR))
        {
            analysisResults.Clear();
        }
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndVertical();
    }

    private bool HasResults()
    {
        if (analysisResults.Count == 0)
        {
            return false;
        }

        foreach(List<string> res in analysisResults.Values)
        {
            if(res.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void DrawResult()
    {
        if(!HasResults())
        {
            EditorGUILayout.LabelField(LocalizationsEditorTexts.NO_RESULTS);
        }

        foreach (KeyValuePair<AnalysisMode, List<string>> res in analysisResults)
        {
            foreach (string element in res.Value)
            {
                GUI.backgroundColor = Color.red;
                EditorGUILayout.BeginHorizontal("HelpBox");
                EditorGUILayout.LabelField(element, BoldStyle, GUILayout.Width(180));
                EditorGUILayout.LabelField(analysisLogs[res.Key]);
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }
        }
    }

    private void ExecuteAnalysis()
    {
        bool searchedUsages = false;
        if(analysisMode.HasFlag(AnalysisMode.ElementsWithoutUse))
        {
            searchedUsages = TrySearchAllUsages(false);
        }


        analysisResults.Clear();
        foreach(AnalysisMode mode in Enum.GetValues(typeof(AnalysisMode)))
        {
            if(analysisMode.HasFlag(mode))
            {
                analysisResults[mode] = new List<string>();
            }
        }

        for(int i = 0; i < _editorDataWrapper.AllLocalizationsData.Count; i++)
        {
            float analysisProgress = (float)i / _editorDataWrapper.AllLocalizationsData.Count;
            LocalizationsEditorData data = _editorDataWrapper.AllLocalizationsData[i];

            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.ANALYZING_PROGRESS_TITLE, LocalizationsEditorTexts.ANALYZING_PROGRESS_GROUP.Replace("{0}", data.group), analysisProgress);

            EditorUtility.DisplayProgressBar(LocalizationsEditorTexts.ANALYZING_PROGRESS_TITLE, LocalizationsEditorTexts.ANALYZING_PROGRESS_KEY.Replace("{0}", data.key), analysisProgress);

            if (analysisMode.HasFlag(AnalysisMode.NotTranslatedKeys) && !data.AreAllLanguagesTranslated)
            {
                analysisResults[AnalysisMode.NotTranslatedKeys].Add(data.key);
            }
            if (analysisMode.HasFlag(AnalysisMode.RepeatedElements) && _editorDataWrapper.IsRepeated(data.key))
            {
                analysisResults[AnalysisMode.RepeatedElements].Add(data.key);
            }
            if(searchedUsages && analysisMode.HasFlag(AnalysisMode.ElementsWithoutUse) && usagesResults[data.key].Count == 0)
            {
                analysisResults[AnalysisMode.ElementsWithoutUse].Add(data.key);
            }

        }

        EditorUtility.ClearProgressBar();
    }
}
