using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextMeshLocalizer : TextLocalizer
{
    protected override void SetLocalization(string translation)
    {
        if (TryGetComponent<TextMesh>(out TextMesh component))
        {
            component.text = translation;
        }
    }
}
