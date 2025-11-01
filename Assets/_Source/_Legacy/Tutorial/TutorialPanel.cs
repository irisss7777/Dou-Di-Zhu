using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    void Start()
    {
        Time.timeScale = 0;
    }

    public void DisablePanel()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
    }
}
