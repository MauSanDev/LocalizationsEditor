using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class TextMeshProLocalizer : TextLocalizer
{
    protected override void SetLocalization(string translation)
    {
        if (TryGetComponent<TextMeshPro>(out TextMeshPro component))
        {
            component.text = translation;
        }
    }
}
