using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public event EventHandler<bool> GamePauseStateChanged;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        IsPaused = false;
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        GamePauseStateChanged?.Invoke(this, IsPaused);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        GamePauseStateChanged?.Invoke(this, IsPaused);
    }
}