using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class LocalizationsEditor
{
    private Vector2 sidebarScroll = Vector2.zero;
    private string sidebarSearch = null;
    private SidebarSearchType sidebarSearchType = SidebarSearchType.Keys;
    private int selectedGroupIndex = -1;
    private int selectedSubGroupIndex = -1;
    
    private Dictionary<string, bool> foldoutGroupStatus = new Dictionary<string, bool>();
    private Dictionary<string, Dictionary<string, bool>> foldoutSubGroupStatus = new Dictionary<string, Dictionary<string, bool>>();
    
    private enum SidebarSearchType
    {
        Keys,
        Groups
    }
    
    private bool IsSearchResult(string textToSearch, string origin) => string.IsNullOrEmpty(textToSearch) || origin.ToLower().Contains(textToSearch.ToLower());
    
    private void DrawSidebar()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.3f));

        GUI.backgroundColor = Color.black;
        
        EditorGUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField(LocalizationsEditorTexts.SIDEBAR_SEARCH, GUILayout.Width(60));
        sidebarSearch = EditorGUILayout.TextField(sidebarSearch);

        GUI.backgroundColor = Color.white;
        
        EditorGUI.BeginChangeCheck();
        sidebarSearchType = (SidebarSearchType) EditorGUILayout.EnumPopup(sidebarSearchType, GUILayout.Width(60f));
        if (EditorGUI.EndChangeCheck())
        {
            foldoutGroupStatus.Clear();
            foldoutSubGroupStatus.Clear();
            if (sidebarSearchType == SidebarSearchType.Groups)
            {
                foreach (string group in groups)
                {
                    foldoutGroupStatus.Add(group, false);
                    foldoutSubGroupStatus.Add(group, new Dictionary<string, bool>());
                    
                    foreach (string subgroup in subgroups[group])
                    {
                        foldoutSubGroupStatus[group].Add(subgroup, false);
                    }
                }
            }
        }
        
        
        EditorGUILayout.EndHorizontal();

        sidebarScroll = EditorGUILayout.BeginScrollView(sidebarScroll);
    
        switch(sidebarSearchType)
        {
            case SidebarSearchType.Keys:
                DrawSidebarByKeys();
                break;
            case SidebarSearchType.Groups:
                DrawSidebarByGroups();
                break;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    

    private void DrawSidebarByGroups()
    {
        if (groups.Length == 0)
        {
            EditorGUILayout.LabelField(LocalizationsEditorTexts.SIDEBAR_NO_GROUPS);
            return;
        }

        for (int i = 0; i < groups.Length; i++)
        {
            if (!IsSearchResult(sidebarSearch, groups[i]))
            {
                continue;
            }
            
            
            foldoutGroupStatus[groups[i]] = EditorGUILayout.Foldout(foldoutGroupStatus[groups[i]], groups[i]);

            if (foldoutGroupStatus[groups[i]])
            {
                EditorGUI.indentLevel++;
                for (int j = 0; j < subgroups[groups[i]].Length; j++)
                {
                    foldoutSubGroupStatus[groups[i]][subgroups[groups[i]][j]] = EditorGUILayout.Foldout(foldoutSubGroupStatus[groups[i]][subgroups[groups[i]][j]], subgroups[groups[i]][j]);

                    if (foldoutSubGroupStatus[groups[i]][subgroups[groups[i]][j]])
                    {
                        EditorGUI.indentLevel++;
                        List<LocalizationsEditorData> localizationList = fullStructure[groups[i]][subgroups[groups[i]][j]];
                        
                        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.label) { margin = { right = 0}, padding = { left = 30}});
                        for (int k = 0; k < localizationList.Count; k++)
                        {
                            LocalizationsEditorData loc = localizationList[k];

                            GUI.color = SelectedLocalizationsEditor.key == loc.key ? Color.green : Color.white;

                            if (GUILayout.Button(loc.key, new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Normal, fontSize = 11}))
                            {
                                SelectLocalization(loc);
                            }
        
                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel--;
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
        }
    }

    private void DrawSidebarByKeys()
    {
        if (_editorDataWrapper.AllLocalizationsData.Count == 0)
        {
            EditorGUILayout.LabelField(LocalizationsEditorTexts.SIDEBAR_NO_KEYS);
            return;
        }

        for (int i = 0; i < _editorDataWrapper.AllLocalizationsData.Count; i++)
        {
            LocalizationsEditorData loc = _editorDataWrapper.AllLocalizationsData[i];

            if (!IsSearchResult(sidebarSearch, loc.key))
            {
                continue;
            }

            GUI.backgroundColor = selectedLocalizationIndex == i ? Color.green : Color.white;
            if (GUILayout.Button(loc.key, EditorStyles.toolbarButton))
            {
                SelectLocalization(i);
            }

            GUI.backgroundColor = Color.white;
        }
    }
}
