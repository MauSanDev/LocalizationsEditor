using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProUGUILocalizer : TextLocalizer
{
    protected override void SetLocalization(string translation)
    {
        if (TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI component))
        {
            component.text = translation;
        }
    }
}
