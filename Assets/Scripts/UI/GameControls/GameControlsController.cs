using UnityEngine;

public class GameControlsController : MonoBehaviour
{
    void Start()
    {
        CloseAll();

        GameStateManager.Instance.GamePauseStateChanged += GamePauseStateChanged;
    }

    private void GamePauseStateChanged(object sender, bool isPaused)
    {
        if(isPaused)
            CloseAll();
        else
            ShowAll();
    }
    
    public void ShowAll()
    {
        foreach(Transform child in transform)
            child.gameObject.SetActive(true);
    }

    public void CloseAll()
    {
        foreach(Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
