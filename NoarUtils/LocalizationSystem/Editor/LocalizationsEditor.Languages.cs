using UnityEngine;
using UnityEditor;

public partial class LocalizationsEditor
{
    private void DrawLanguages()
    {
        EditorGUILayout.LabelField(LocalizationsEditorTexts.LANG_TAB_TITL, TitleStyle);
        EditorGUILayout.LabelField(LocalizationsEditorTexts.LANG_TAB_DESC, SmallStyle);

        EditorGUI.BeginChangeCheck();
        chosenDefLang = (SystemLanguage)EditorGUILayout.EnumPopup(LocalizationsEditorTexts.LANG_PRIMARY_LANGUAGE, chosenDefLang);
        if(EditorGUI.EndChangeCheck())
        {
            chosenSysLangs.Remove(chosenDefLang);
        }

        EditorGUILayout.Space();

        int counter = 0;
        EditorGUILayout.BeginHorizontal();
        foreach (SystemLanguage lang in EnumUtil.GetValues<SystemLanguage>())
        {
            if(counter % 4 == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            bool isPrimaryLang = chosenDefLang == lang;
            bool isAvailable = chosenSysLangs.Contains(lang);
            GUI.backgroundColor = isPrimaryLang ? Color.gray : isAvailable ? Color.green : Color.white;
            if(GUILayout.Button($"{(isAvailable ? "✓ " : "")}{lang}{(isPrimaryLang ? LocalizationsEditorTexts.LANG_PRIMARY_LABEL : "")}", GUILayout.Width(160)))
            {
                if(!isPrimaryLang)
                {
                    if (chosenSysLangs.Contains(lang))
                    {
                        chosenSysLangs.Remove(lang);
                    }
                    else
                    {
                        chosenSysLangs.Add(lang);
                    }
                }
            }
            counter++;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUI.backgroundColor = Color.gray;
        if(GUILayout.Button(LocalizationsEditorTexts.LANG_APPLY_BUTTON))
        {
            int result = EditorUtility.DisplayDialogComplex(LocalizationsEditorTexts.LANG_CHANGE_TITLE,LocalizationsEditorTexts.LANG_CHANGE_BODY , LocalizationsEditorTexts.CONTINUE, LocalizationsEditorTexts.CANCEL, "");

            if(result == 0)
            {
                LocalizationSystem.SetDefaultLanguage(chosenDefLang);
                _editorDataWrapper.OverrideLanguages(chosenDefLang, chosenSysLangs);
                EditorUtility.DisplayDialog(LocalizationsEditorTexts.LANG_MODIFIED_TITLE, LocalizationsEditorTexts.LANG_MODIFIED_BODY , "Continue");
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.LabelField(LocalizationsEditorTexts.LANG_CHANGES_WILL_APPLY, SmallStyle);

        EditorGUI.indentLevel--;
    }

}
