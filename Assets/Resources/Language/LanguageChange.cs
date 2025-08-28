using UnityEngine;
using I2.Loc;

public class LanguageChange : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SetLanguage("English");
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            SetLanguage("Chinese");
        }
    }

    void SetLanguage(string lang)
    {
        if (LocalizationManager.HasLanguage(lang, true))
        {
            LocalizationManager.CurrentLanguage = lang;
            Debug.Log($"”Ô—‘«–ªªŒ™: {lang}");
        }
        else
        {
            Debug.LogWarning($"”Ô—‘≤ª¥Ê‘⁄: {lang}");
        }
    }
}
