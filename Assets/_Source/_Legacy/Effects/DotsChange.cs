using TMPro;
using UnityEngine;
using System.Collections;

public class AnimatedDotsText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float delayBetweenSteps = 0.5f;
    public int maxDots = 5;

    private void OnEnable()
    {
        StartCoroutine(AnimateText());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator AnimateText()
    {
        while (true)
        {
            for (int i = 1; i <= maxDots; i++)
            {
                textComponent.text = new string('.', i);
                yield return new WaitForSeconds(delayBetweenSteps);
            }
        }
    }
}