using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour
{
    private HandManager handManager;

    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
    }

    public void InvokeSelect()
    {
        if (handManager != null)
        {
            if (handManager.IsChosen(gameObject))
                handManager.ResetChosenCard(gameObject);
            else
                handManager.SelectCard(gameObject);
        }
    }
}