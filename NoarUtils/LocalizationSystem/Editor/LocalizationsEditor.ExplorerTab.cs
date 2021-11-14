using UnityEditor;
using UnityEngine;
using System.IO;

public partial class LocalizationsEditor
{
    private void DrawExplorer()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_KEY, BoldStyle);

        string usagesAmount = usagesResults.ContainsKey(SelectedLocalizationsEditor.key) ? usagesResults[SelectedLocalizationsEditor.key].Count.ToString() : "??";
        EditorGUILayout.BeginHorizontal("HelpBox", GUILayout.Width(30));
        EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_USAGES + usagesAmount, SmallStyle, GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.gray;
        if(GUILayout.Button(LocalizationsEditorTexts.EXPLORER_SEARCH_USAGES, EditorStyles.miniButtonLeft, GUILayout.Width(120)))
        {
            SearchUsages(SelectedLocalizationsEditor.key);
        }
        if(GUILayout.Button(LocalizationsEditorTexts.EXPLORER_COPY_KEY, EditorStyles.miniButtonMid, GUILayout.Width(80)))
        {
            GUIUtility.systemCopyBuffer = SelectedLocalizationsEditor.key;
        }
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button(LocalizationsEditorTexts.DELETE, EditorStyles.miniButtonRight, GUILayout.Width(80)))
        {
            TryDeleteRegistry(SelectedLocalizationsEditor);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        SelectedLocalizationsEditor.key = EditorGUILayout.TextField(SelectedLocalizationsEditor.key, TitleStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if(_editorDataWrapper.AllKeys.Contains(SelectedLocalizationsEditor.key))
            {
                GUI.backgroundColor = Color.red;
                EditorGUILayout.BeginHorizontal("HelpBox");
                EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_KEY_EXISTS, SmallStyle);
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }
        }

        EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_DESCRIPTION, SmallStyle);
        SelectedLocalizationsEditor.description = EditorGUILayout.TextField(SelectedLocalizationsEditor.description, SmallStyle);
        
        EditorGUILayout.Space();

        bool hasGroupSettled = !string.IsNullOrEmpty(SelectedLocalizationsEditor.group);
        bool hasSubGroupSettled = !string.IsNullOrEmpty(SelectedLocalizationsEditor.subGroup);

        if (!hasGroupSettled)
        {
            createNewGroup = true;
            createNewSubGroup = true;
        }

        EditorGUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.CREATE_NEW_GROUP, SmallStyle, GUILayout.Width(80));
        EditorGUI.BeginDisabledGroup(!hasGroupSettled);
        EditorGUI.BeginChangeCheck();
        createNewGroup = EditorGUILayout.Toggle(createNewGroup, GUILayout.Width(80));
        if(EditorGUI.EndChangeCheck())
        {
            createNewSubGroup = createNewGroup;
        
            shouldCreateSubGroup = createNewGroup || !hasGroupSettled || !hasSubGroupSettled;
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
            SelectedLocalizationsEditor.group = EditorGUILayout.TextField(LocalizationsEditorTexts.CREATE_GROUP, SelectedLocalizationsEditor.@group);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            newElementSelectedGroupIndex = EditorGUILayout.Popup(LocalizationsEditorTexts.CREATE_GROUP, newElementSelectedGroupIndex, groups);
            if (EditorGUI.EndChangeCheck())
            {
                SelectedLocalizationsEditor.subGroup = groups[newElementSelectedGroupIndex];
            }
        }

        if(createNewSubGroup)
        {
            SelectedLocalizationsEditor.subGroup = EditorGUILayout.TextField(LocalizationsEditorTexts.CREATE_SUBGROUP, SelectedLocalizationsEditor.subGroup);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            newElementSelectedSubGroupIndex = EditorGUILayout.Popup(LocalizationsEditorTexts.CREATE_SUBGROUP, newElementSelectedSubGroupIndex, subgroups[groups[newElementSelectedGroupIndex]]);
            if (EditorGUI.EndChangeCheck())
            {
                SelectedLocalizationsEditor.subGroup = subgroups[groups[newElementSelectedGroupIndex]][newElementSelectedSubGroupIndex];
            }
        }
        
        EditorGUILayout.Space();

        GUI.backgroundColor = Color.black;
        
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_LANG, BoldStyle, GUILayout.Width(130));
        EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_TRANSLATION, BoldStyle);
        EditorGUILayout.EndHorizontal();
        
        for (int i = 0; i < SelectedLocalizationsEditor.languages.Count; i++)
        {
            GUI.backgroundColor = i % 2 == 0 ? Color.gray : Color.white;
            EditorGUILayout.BeginHorizontal("box");
            SelectedLocalizationsEditor.translations[i] = EditorGUILayout.TextField(SelectedLocalizationsEditor.languages[i] + ":", SelectedLocalizationsEditor.translations[i], EditorStyles.label);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button(LocalizationsEditorTexts.SAVE))
        {
            SaveAssets();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_USAGES, TitleStyle);
        EditorGUILayout.BeginVertical("Box");

        if (usagesResults.ContainsKey(SelectedLocalizationsEditor.key))
        {
            if (usagesResults[SelectedLocalizationsEditor.key].Count > 0)
            {
                foreach (string reference in usagesResults[SelectedLocalizationsEditor.key])
                {
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(reference.Replace(Application.dataPath, "Assets/"));
                    EditorGUILayout.BeginHorizontal("HelpBox");
                    if (GUILayout.Button(Path.GetFileName(reference), new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft }, GUILayout.Width(250)))
                    {
                        PingObject(asset);
                    }

                    EditorGUILayout.LabelField($" ({asset.GetType()} - {asset.GetType().BaseType})", new GUIStyle() { normal = { textColor = Color.gray }, fontStyle = FontStyle.Italic });

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_NO_USAGES);
            }
        }
        else
        {
            EditorGUILayout.LabelField(LocalizationsEditorTexts.EXPLORER_USAGES_NOT_SEARCHED);
        }

        EditorGUILayout.EndVertical();
    }
}
