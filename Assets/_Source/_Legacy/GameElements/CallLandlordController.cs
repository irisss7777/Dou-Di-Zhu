using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallLandlordButtonsController : MonoBehaviour
{
    [SerializeField] private GameManagerLLS gameManagerLLS;

    void Start()
    {
        if(!gameManagerLLS)
            gameManagerLLS = FindObjectOfType<GameManagerLLS>();
    }

    public void Call()
    {
        gameManagerLLS.Call();
    }

    public void Pass()
    {
        gameManagerLLS.Pass();
    }
}
