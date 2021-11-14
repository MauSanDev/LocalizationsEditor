using UnityEngine;

public abstract class TextLocalizer : MonoBehaviour
{
    [SerializeField] protected string key = null;

    private void Awake()
    {
        SetLocalization(LocalizedText);
    }
    
    public string Key
    {
        get => key;
        set => key = value;
    }

    public string LocalizedText => LocalizationSystem.Localize(key);

    protected abstract void SetLocalization(string translation);
}
