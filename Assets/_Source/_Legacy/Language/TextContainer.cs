using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextContainer : MonoBehaviour
{
    public string key;
    private TextMeshPro tmpText;
    private TextMeshProUGUI tmpTextUGUI;
    private LanguageManager languageManager;

    private TMP_FontAsset originalFontAsset;
    void Start()
    {
        tmpText = gameObject.GetComponent<TextMeshPro>();
        if (tmpText != null)
            originalFontAsset = tmpText.font;
        tmpTextUGUI = gameObject.GetComponent<TextMeshProUGUI>();
        if (tmpTextUGUI != null)
            originalFontAsset = tmpTextUGUI.font;
        languageManager = FindObjectOfType<LanguageManager>();
        if (languageManager)
        {
            UpdateText(languageManager.CurrentLanguage);
            languageManager.OnLanguageUpdate += UpdateText;
        }
    }

    private void UpdateText(Dictionary<string, string> language)
    {
        if (tmpText != null)
        {
            try
            {
                tmpText.text = language[key];
                tmpText.font = language["language"] == "chinese" ? languageManager.chineseFontAsset : originalFontAsset;
            }
            catch
            {
                Debug.Log("Невозможно изменить текст!");
            }
        }

        if (tmpTextUGUI != null)
        {
            try
            {
                tmpTextUGUI.text = language[key];
                tmpTextUGUI.font = language["language"] == "chinese" ? languageManager.chineseFontAsset : originalFontAsset;
            }
            catch
            {
                Debug.Log("Невозможно изменить текст!");
            }
        }
    }

    void OnDestroy()
    {
        if (languageManager)
        {
            languageManager.OnLanguageUpdate -= UpdateText;
        }
    }
}
