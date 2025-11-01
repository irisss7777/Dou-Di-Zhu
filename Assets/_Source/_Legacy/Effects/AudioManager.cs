using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    void Start()
    {
        try
        {
            audioSource.volume = FindObjectOfType<WebGameManager>().userStats.soundValue;
        }
        catch
        {

        }
    }
}
