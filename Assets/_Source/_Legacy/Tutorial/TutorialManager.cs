using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float initialDelay;
    [SerializeField] private float delayBetweenMessages;
    [SerializeField] private float secondPartDelay;
    [SerializeField] private float thirdPartDelay;
    [SerializeField] private GameObject[] firstPart;
    [SerializeField] private GameObject[] secondPart;
    [SerializeField] private GameObject[] thirdPart;
    private WebGameManager webGameManager;

    private void Start() 
    {
        webGameManager = FindObjectOfType<WebGameManager>();
        if(!webGameManager || !webGameManager.isTutorial)
        {
            this.gameObject.SetActive(false);
            return;
        }
        webGameManager.OnLandlord += (long val1, int val2) => 
        {
            StartCoroutine(ShowThirdPart());
        };
        StartCoroutine(ShowFirstPart());
    }

    private IEnumerator ShowFirstPart()
    {
        yield return new WaitForSeconds(initialDelay);
        foreach(GameObject item in firstPart)
        {
            item.SetActive(true);
            yield return new WaitForSeconds(delayBetweenMessages);
        }
        yield return new WaitForSeconds(secondPartDelay);
        foreach(GameObject item in secondPart)
        {
            item.SetActive(true);
            yield return new WaitForSeconds(delayBetweenMessages);
        }
    }

    private IEnumerator ShowThirdPart()
    {
         yield return new WaitForSeconds(thirdPartDelay);
        foreach(GameObject item in thirdPart)
        {
            item.SetActive(true);
            yield return new WaitForSeconds(delayBetweenMessages);
        }
    }
}
