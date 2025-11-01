using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public static class WebglBridge
{
    [DllImport("__Internal")]
    private static extern void OnGameLoaded();

    [DllImport("__Internal")]
    private static extern void OnGameStarted();

    [DllImport("__Internal")]
    private static extern void OnTimed();

    [DllImport("__Internal")]
    private static extern void OnGameExit();

    [DllImport("__Internal")]
    private static extern void OnScoreUpdate(bool isWin, int scoreAdded, int scoreTotal);
    
    public static void SendLoadedEvent()
    {
#if UNITY_WEBGL && ! UNITY_EDITOR
        OnGameLoaded();
#endif
    }

    public static void SendStartEvent()
    {
#if UNITY_WEBGL && ! UNITY_EDITOR
        OnGameStarted();
#endif
    }

    public static void SendTimedEvent()
    {
#if UNITY_WEBGL && ! UNITY_EDITOR
        OnTimed();
#endif
    }
    
    public static void SendExitEvent()
    {
#if UNITY_WEBGL && ! UNITY_EDITOR
        OnGameExit();
#endif
    }

    public static void SendScoreUpdate(bool isWin, int scoreAdded, int scoreTotal)
    {
#if UNITY_WEBGL && ! UNITY_EDITOR
        OnScoreUpdate(isWin, scoreAdded, scoreTotal);
#endif
    }
}