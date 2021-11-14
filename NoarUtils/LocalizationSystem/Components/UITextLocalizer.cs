using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITextLocalizer : TextLocalizer
{
    protected override void SetLocalization(string translation)
    {
        if (TryGetComponent<Text>(out Text component))
        {
            component.text = translation;
        }
    }
}
