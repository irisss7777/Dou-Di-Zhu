using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManagerFunctionsProvider : MonoBehaviour
{
    LanguageManager languageManager;

    void Start()
    {
        languageManager = FindObjectOfType<LanguageManager>();
    }

    public void SetLanguageToEnglish()
    {
        if (languageManager)
        {
            languageManager.SetLanguageToEnglish();
        }
    }

    public void SetLanguageToRussian()
    {
        if (languageManager)
        {
            languageManager.SetLanguageToRussian();
        }
    }

    public void SetLanguageToChinese()
    {
        if (languageManager)
        {
            languageManager.SetLanguageToChinese();
        }
    }
}
