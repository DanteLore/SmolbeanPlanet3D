using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsStarted { get; private set;}
    public event EventHandler<float> GameSpeedChanged;
    public event EventHandler<bool> GamePauseStateChanged;
    public event EventHandler<bool> GameStatusChanged;
    private float selectedGameSpeed = 1.0f;
    public float SelectedGameSpeed 
    {
        get { return selectedGameSpeed; }
        set
        {
            selectedGameSpeed = value;
            GameSpeedChanged?.Invoke(this, selectedGameSpeed);
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

    void OnApplicationQuit()
    {
        EndGame();
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
        Time.timeScale = SelectedGameSpeed;
        AudioListener.pause = false;
        GamePauseStateChanged?.Invoke(this, IsPaused);
    }    

    public void EndGame()  
    { 
        IsStarted = false;
        AudioListener.pause = true;
        GameStatusChanged?.Invoke(this, IsStarted);
    }

    public void StartGame()
    {
        IsStarted = true;
        SelectedGameSpeed = 1f;
        AudioListener.pause = false;
        GameStatusChanged?.Invoke(this, IsStarted);
    }

    public void Update()
    {
        if(IsPaused)
            return;

        if(Input.GetKeyDown("1"))
            SelectedGameSpeed = 1.0f;
        else if(Input.GetKeyDown("2"))
            SelectedGameSpeed = 2.0f;
        else if(Input.GetKeyDown("3"))
            SelectedGameSpeed = 4.0f;
        else if(Input.GetKeyDown("4"))
            SelectedGameSpeed = 8.0f;

        if(SelectedGameSpeed != Time.timeScale)
            Time.timeScale = SelectedGameSpeed;
    }
}