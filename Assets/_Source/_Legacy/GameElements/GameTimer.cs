using System.Collections;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Ссылка на TextMeshPro
    public TextMeshPro timerText2; // Ссылка на TextMeshPro
    public Transform timerTransform;  // Объект для анимации
    public float shakeAmount = 5f;    // Амплитуда тряски
    [SerializeField] private int normalTime = 20;
    [SerializeField] private int tutorialTime = 50;
    [SerializeField] private int shortTime = 10;
    private int timeLeft = 20;        // Оставшееся время
    private bool isPassedPrevTurn = false;
    private Coroutine timerCoroutine;
    private Vector3 startScale = Vector3.zero;

    [SerializeField] private bool isMainTimer = true;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManagerLLS gameManagerLLS;
    [SerializeField] private GameManagerMG gameManagerMG;
    [SerializeField] private AIPlayer aiPlayerOptional;
    private Vector3 originalScale;

    private WebGameManager webGameManager;

    void Awake()
    {
        webGameManager = FindObjectOfType<WebGameManager>();
        originalScale = timerTransform.localScale;
    }

    public void StartTimer()
    {
        if (startScale != Vector3.zero)
            timerTransform.localScale = startScale;
        else
            startScale = timerTransform.localScale;
        timerTransform.rotation = Quaternion.identity;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timeLeft = isPassedPrevTurn ? shortTime : (webGameManager && webGameManager.isTutorial && isMainTimer ? tutorialTime : normalTime); 
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            isPassedPrevTurn = false;
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (timeLeft > 0)
        {
            if(timerText)
                timerText.text = timeLeft.ToString();
            if(timerText2)
                timerText2.text = timeLeft.ToString();

            if (timeLeft > 10)
            {
                StartCoroutine(PulseEffect());
            }
            else
            {
                StartCoroutine(ShakeEffect());
            }

            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        if(timerText)
            timerText.text = "0";
        if(timerText2)
            timerText2.text = "0";
        if(isMainTimer)
        {
            if (webGameManager && !webGameManager.isMultiplayer && aiPlayerOptional)
            {
                if (gameManager.gameState == GameState.chooseLandlord)
                    gameManagerLLS.Pass();
                if (gameManager.gameState == GameState.game)
                    aiPlayerOptional.MakeTurnForMainPlayer();
                isPassedPrevTurn = true;
            }
        }
    }

    private IEnumerator PulseEffect()
    {
        Vector3 targetScale = originalScale * 1.2f;

        float elapsedTime = 0f;
        float duration = 0.2f;
        while (elapsedTime < duration)
        {
            timerTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            timerTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 originalRotation = timerTransform.eulerAngles;

        float elapsedTime = 0f;
        float duration = 0.2f;
        while (elapsedTime < duration)
        {
            float shakeOffset = Mathf.Sin(Time.time * 50) * shakeAmount;
            timerTransform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, shakeOffset);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        timerTransform.eulerAngles = originalRotation;
    }
}