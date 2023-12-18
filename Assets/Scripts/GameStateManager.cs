using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsStarted { get; private set;}
    public event EventHandler<float> GameSpeedChanged;
    public event EventHandler<bool> GamePauseStateChanged;
    public event EventHandler<bool> GameStarted;
    private float selectedTimeScale = 1.0f;
    public float SelectedTimeScale 
    {
        get { return selectedTimeScale; }
        private set
        {
            selectedTimeScale = value;
            Debug.Log("Time scale set to: " + selectedTimeScale);
            GameSpeedChanged?.Invoke(this, selectedTimeScale);
        }
    }

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
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        GamePauseStateChanged?.Invoke(this, IsPaused);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = SelectedTimeScale;
        AudioListener.pause = false;
        GamePauseStateChanged?.Invoke(this, IsPaused);
    }

    public void StartGame()
    {
        IsStarted = true;
        GameStarted?.Invoke(this, IsStarted);
    }

    public void Update()
    {
        if(IsPaused)
            return;

        if(Input.GetKeyDown("1"))
            SelectedTimeScale = 1.0f;
        else if(Input.GetKeyDown("2"))
            SelectedTimeScale = 2.0f;
        else if(Input.GetKeyDown("3"))
            SelectedTimeScale = 4.0f;
        else if(Input.GetKeyDown("4"))
            SelectedTimeScale = 8.0f;

        if(SelectedTimeScale != Time.timeScale)
            Time.timeScale = SelectedTimeScale;
    }
}