using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsStarted { get; private set;}
    public event EventHandler<bool> GamePauseStateChanged;
    public event EventHandler<bool> GameStarted;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        IsPaused = false;
        IsStarted = false;
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

    public void StartGame()
    {
        IsStarted = true;
        GameStarted?.Invoke(this, IsStarted);
    }
}