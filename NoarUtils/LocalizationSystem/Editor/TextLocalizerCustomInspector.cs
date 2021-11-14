using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using System;

[CustomEditor(typeof(TextLocalizer), true)]
[CanEditMultipleObjects]
public class TextLocalizerCustomInspector : Editor
{
    private const string KEY_DOESNT_EXIST_PREVIEW = "[The settled key doesn't exists!]";
    private const string LANGUAGE_UNSUPPORTED_PREVIEW = "[The settled preview language is not available.]";
    private const string LANG_NOT_TRANSLATED_PREVIEW = "[The settled key is not translated to this language.]";

    private TextLocalizer baseComp = null;
    private LocalizationsEditorDataWrapper database = null;
    private bool keyExists = false;
    private SearchField searchField = null;
    private string[] availableLanguages = null;
    private int selectedPreviewLangIndex = 0;
    private bool previewOnComponent = true;

    private MethodInfo method = null;

    private void OnEnable()
    {
        baseComp = target as TextLocalizer;
        database = LocalizationsEditor.LoadLocalizationsWrapper();
        method = baseComp.GetType().GetMethod("SetLocalization", BindingFlags.NonPublic | BindingFlags.Instance);

        keyExists = database.ContainsKey(baseComp.Key);
        searchField = new SearchField();
        UpdateAvailableLanguages();
    }

    private void UpdateAvailableLanguages()
    {
            availableLanguages = new string[database.AvailableLanguages.Count];

            for(int i = 0; i < database.AvailableLanguages.Count; i++)
            {
                availableLanguages[i] = database.AvailableLanguages[i].ToString();
            }
    }

    private void DrawEditorButton()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Text Localizer:", BoldStyle);

        GUI.backgroundColor = previewOnComponent ? Color.green : Color.black;
        if (GUILayout.Button($"{(previewOnComponent ? "✔ " : "")}Preview Changes", EditorStyles.miniButtonLeft, GUILayout.Width(140)))
        {
            previewOnComponent = !previewOnComponent;
            SetLocalizationOnComponent();
        }
        GUI.backgroundColor = Color.black;
        if (GUILayout.Button("Open Editor", EditorStyles.miniButtonRight, GUILayout.Width(100)))
        {
            LocalizationsEditor.ShowLocalizationsEditor();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        DrawEditorButton();

        GUI.backgroundColor = keyExists ? Color.white : Color.red;
        EditorGUILayout.BeginHorizontal("Box");

        EditorGUI.BeginChangeCheck();

        //TODO: To make it more useful we should replace it with a TreeView inside a Rect and show all the available keys there!
        baseComp.Key = searchField.OnGUI(GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true)), baseComp.Key);

        if (EditorGUI.EndChangeCheck())
        {
            SetLocalizationOnComponent();
            keyExists = database.ContainsKey(baseComp.Key);
        }

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.BeginVertical("HelpBox");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Preview:", BoldStyle, GUILayout.Width(70));

        EditorGUI.BeginChangeCheck();
        selectedPreviewLangIndex = EditorGUILayout.Popup(selectedPreviewLangIndex, availableLanguages);
        if(EditorGUI.EndChangeCheck())
        {
            SetLocalizationOnComponent();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(PreviewText, ItalicStyle);

        EditorGUILayout.EndVertical();
    }

    private SystemLanguage SelectedPreviewLanguage => (SystemLanguage) Enum.Parse(typeof(SystemLanguage), availableLanguages[selectedPreviewLangIndex]);

    private string PreviewText
    {
        get
        {
            if (!database.AvailableLanguages.Contains(SelectedPreviewLanguage))
            {
                return LANGUAGE_UNSUPPORTED_PREVIEW;
            }

            string localized = database.Localize(baseComp.Key, SelectedPreviewLanguage);

            if (baseComp.Key == localized)
            {
                if(database.ContainsKey(baseComp.Key))
                {
                    return LANG_NOT_TRANSLATED_PREVIEW;
                }
                
                return KEY_DOESNT_EXIST_PREVIEW;
            }

            return localized;
        }
    }

    private void SetLocalizationOnComponent()
    {
        if(!previewOnComponent || method == null)
        {
            return;
        }
        
        method.Invoke(baseComp, new object[] { database.Localize(baseComp.Key, SelectedPreviewLanguage) });
        EditorUtility.SetDirty(baseComp);
    }


    private GUIStyle BoldStyle => new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
    private GUIStyle ItalicStyle => new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic };
}
