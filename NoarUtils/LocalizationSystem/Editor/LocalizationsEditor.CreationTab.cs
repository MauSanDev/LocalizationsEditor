using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class LocalizationsEditor
{
    private bool shouldCreateSubGroup = false;
    
    private void DrawCreationTab()
    {
        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_TAB_TITLE, TitleStyle);
        
        EditorGUILayout.Space();

        _placeholderLocalizationsEditorData.key = EditorGUILayout.TextField(LocalizationsEditorTexts.CREATE_KEY, _placeholderLocalizationsEditorData.key);
        _placeholderLocalizationsEditorData.description = EditorGUILayout.TextField(LocalizationsEditorTexts.CREATE_DESC, _placeholderLocalizationsEditorData.description);

        EditorGUILayout.Space();

        GUI.backgroundColor = Color.gray;
        EditorGUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_NEW_GROUP, SmallStyle, GUILayout.Width(80));

        creationExceptionsDic[LocalizationsEditorTexts.ERROR_EMPTY_KEY] = string.IsNullOrEmpty(_placeholderLocalizationsEditorData.key);
        creationExceptionsDic[LocalizationsEditorTexts.ERROR_KEY_HAS_WHITESPACES] = _placeholderLocalizationsEditorData.key.Contains(" ");
        creationExceptionsDic[LocalizationsEditorTexts.ERROR_REPEATED_KEY] = _editorDataWrapper.ContainsKey(_placeholderLocalizationsEditorData.key);

        bool hasGroups = groups.Length > 0;
        bool hasSubGroups = subgroups[groups[newElementSelectedGroupIndex]].Length > 0;

        if (!hasGroups)
        {
            createNewGroup = true;
            createNewSubGroup = true;
        }

        EditorGUI.BeginDisabledGroup(!hasGroups);
        EditorGUI.BeginChangeCheck();
        createNewGroup = EditorGUILayout.Toggle(createNewGroup, GUILayout.Width(80));
        if(EditorGUI.EndChangeCheck())
        {
            createNewSubGroup = createNewGroup;
        
            shouldCreateSubGroup = createNewGroup || !hasGroups || !hasSubGroups;
            createNewSubGroup = shouldCreateSubGroup;
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_NEW_SUBGROUP, SmallStyle, GUILayout.Width(100));
        
        EditorGUI.BeginDisabledGroup(shouldCreateSubGroup);
        createNewSubGroup = EditorGUILayout.Toggle(createNewSubGroup, GUILayout.Width(80));
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();

        if(createNewGroup)
        {
            _placeholderLocalizationsEditorData.group = EditorGUILayout.TextField(LocalizationsEditorTexts.CREATE_GROUP, _placeholderLocalizationsEditorData.@group);
        }
        else
        {
            newElementSelectedGroupIndex = EditorGUILayout.Popup(LocalizationsEditorTexts.CREATE_GROUP, newElementSelectedGroupIndex, groups);
        }

        if(createNewSubGroup)
        {
            _placeholderLocalizationsEditorData.subGroup = EditorGUILayout.TextField(LocalizationsEditorTexts.CREATE_SUBGROUP, _placeholderLocalizationsEditorData.subGroup);
        }
        else
        {
            newElementSelectedSubGroupIndex = EditorGUILayout.Popup(LocalizationsEditorTexts.CREATE_SUBGROUP, newElementSelectedSubGroupIndex, subgroups[groups[newElementSelectedGroupIndex]]);
        }

        creationExceptionsDic[LocalizationsEditorTexts.ERROR_GROUP_NOT_DEFINED] = createNewGroup && string.IsNullOrEmpty(_placeholderLocalizationsEditorData.@group);
        creationExceptionsDic[LocalizationsEditorTexts.ERROR_SUBGROUP_NOT_DEFINED] = createNewSubGroup && string.IsNullOrEmpty(_placeholderLocalizationsEditorData.subGroup);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.backgroundColor = Color.black;
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_LANG, BoldStyle, GUILayout.Width(130));
        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_TRANS, BoldStyle);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < _editorDataWrapper.AvailableLanguages.Count; i++)
        {
            bool isPrimaryLanguage = _editorDataWrapper.AvailableLanguages[i] == _editorDataWrapper.PrimaryLanguage;
            if(isPrimaryLanguage)
            {
                creationExceptionsDic[LocalizationsEditorTexts.ERROR_PRIMARY_LANGUAGE_EMPTY] = string.IsNullOrEmpty(_placeholderLocalizationsEditorData.translations[i]);
            }

            GUI.backgroundColor = i % 2 == 0 ? Color.gray : Color.white;
            EditorGUILayout.BeginHorizontal("box");
            _placeholderLocalizationsEditorData.translations[i] = EditorGUILayout.TextField(_editorDataWrapper.AvailableLanguages[i] + ":", _placeholderLocalizationsEditorData.translations[i], EditorStyles.label);
            EditorGUILayout.EndHorizontal();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_EMPTY_FIELDS, SmallStyle);

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(HasCreationExceptions);
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button(LocalizationsEditorTexts.CREATE_BUTTON))
        {
            CreateKey();
        }
        GUI.backgroundColor = Color.white;
        EditorGUI.EndDisabledGroup();

        if (HasCreationExceptions)
        {
            EditorGUILayout.Space();

            GUI.backgroundColor = Color.red;
            EditorGUILayout.BeginVertical("HelpBox");
            foreach (KeyValuePair<string, bool> exception in creationExceptionsDic)
            {
                if (exception.Value)
                {
                    EditorGUILayout.LabelField($"- {exception.Key}", SmallStyle);
                }
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }
    }
    
}
