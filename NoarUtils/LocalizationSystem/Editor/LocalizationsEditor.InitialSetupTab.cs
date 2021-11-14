using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class LocalizationsEditor
{
    private SystemLanguage primaryLanguage = SystemLanguage.English;
    private SystemLanguage chosenDefLang = SystemLanguage.English;
    private List<SystemLanguage> chosenSysLangs = new List<SystemLanguage>();
    
    private void DrawInitialSetupTab()
    {
        EditorGUILayout.LabelField(LocalizationsEditorTexts.INIT_TAB_NAME, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.INIT_TAB_DESC, GUILayout.Height(90));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(LocalizationsEditorTexts.LANG_PRIMARY_LANGUAGE, TitleStyle);
        primaryLanguage = (SystemLanguage) EditorGUILayout.EnumPopup(LocalizationsEditorTexts.LANG_PRIMARY_LANGUAGE, primaryLanguage);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(LocalizationsEditorTexts.INIT_AVAILABLE_LANGS, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.INIT_AVAILABLE_LANGS_DESC, SmallStyle);

        int counter = 0;
        EditorGUILayout.BeginHorizontal();
        foreach (SystemLanguage lang in EnumUtil.GetValues<SystemLanguage>())
        {
            if (counter % 4 == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            bool isPrimary = primaryLanguage == lang;
            bool isAvailable = chosenSysLangs.Contains(lang);
            GUI.backgroundColor = isPrimary ? Color.gray : isAvailable ? Color.green : Color.white;
            if (GUILayout.Button($"{(isAvailable ? "✓ " : "")}{lang}{(isPrimary ? LocalizationsEditorTexts.LANG_PRIMARY_LABEL : "")}", GUILayout.Width(160)))
            {
                if(primaryLanguage == lang)
                {
                    continue;
                }

                if (chosenSysLangs.Contains(lang))
                {
                    chosenSysLangs.Remove(lang);
                }
                else
                {
                    chosenSysLangs.Add(lang);
                }
            }
            counter++;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if(GUILayout.Button(LocalizationsEditorTexts.INIT_START_BUTTON))
        {
            CreateWrapper();
        }
    }
}
