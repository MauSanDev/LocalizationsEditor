using UnityEngine;
using UnityEditor;

public partial class LocalizationsEditor
{
    private const string TOOL_REPOSITORY_URL = "https://github.com/MauSanDev/LocalizationsEditor";
    
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = currentTab == Tabs.Create ? Color.green : Color.white;
        if(GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_CREATE, EditorStyles.toolbarDropDown, GUILayout.Width(70)))
        {
            ShowCreateMenu();
        }
        GUI.backgroundColor = currentTab == Tabs.Languages ? Color.green : Color.white;
        if (GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_SETTINGS, EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            SetTab(Tabs.Languages);
        }
        GUI.backgroundColor = Color.white;
        
        if (GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_IMPORT_EXPORT, EditorStyles.toolbarButton, GUILayout.Width(110)))
        {
            SetTab(Tabs.ImportExport);
        }
        
        if(GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_ANALYZE, EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            SetTab(Tabs.Analyze);
        }
        if(GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_SEARCH_USAGES, EditorStyles.toolbarButton, GUILayout.Width(120)))
        {
            TrySearchAllUsages();
        }
        
        if (GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_BUILD_ASSETS, EditorStyles.toolbarButton, GUILayout.Width(90)))
        {
            BuildAssets();
        }
        
        if(GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_PING, EditorStyles.toolbarDropDown, GUILayout.Width(60)))
        {
            ShowPingMenu();
        }
        
        if (GUILayout.Button(LocalizationsEditorTexts.TOOLBAR_HELP, EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            Application.OpenURL(TOOL_REPOSITORY_URL);
        }

        EditorGUILayout.EndHorizontal();
    }
    
    private void ShowPingMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent(LocalizationsEditorTexts.TOOLBAR_PING_WRAPPER), false, () => PingObject(_editorDataWrapper));
        menu.AddItem(new GUIContent(LocalizationsEditorTexts.TOOLBAR_PING_BUILT_ASSETS), false, TryPingBuiltAssets);
        menu.ShowAsContext();
    }
    
    private void ShowCreateMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent(LocalizationsEditorTexts.TOOLBAR_NEW_LOC), false, () => SetTab(Tabs.Create));
        menu.ShowAsContext();
    }

}
