using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public event EventHandler<bool> OnGamePaused;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Pause()
    {
        IsPaused = true;
        OnGamePaused?.Invoke(this, IsPaused);
    }

    public void Resume()
    {
        IsPaused = false;
        OnGamePaused?.Invoke(this, IsPaused);
    }
}