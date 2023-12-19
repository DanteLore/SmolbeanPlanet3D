using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSpeedControls : MonoBehaviour
{
    Button speed1Button;
    Button speed2Button;
    Button speed3Button;
    Button speed4Button;

    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        
        speed1Button = document.rootVisualElement.Q<Button>("gameSpeed1Button");
        speed1Button.clicked += () => GameStateManager.Instance.SelectedGameSpeed = 1.0f;
        speed2Button = document.rootVisualElement.Q<Button>("gameSpeed2Button");
        speed2Button.clicked += () => GameStateManager.Instance.SelectedGameSpeed = 2.0f;
        speed3Button = document.rootVisualElement.Q<Button>("gameSpeed3Button");
        speed3Button.clicked += () => GameStateManager.Instance.SelectedGameSpeed = 4.0f;
        speed4Button = document.rootVisualElement.Q<Button>("gameSpeed4Button");
        speed4Button.clicked += () => GameStateManager.Instance.SelectedGameSpeed = 8.0f;

        UpdateButtons(GameStateManager.Instance.SelectedGameSpeed);
        GameStateManager.Instance.GameSpeedChanged += GameSpeedChanged;
    }

    private void GameSpeedChanged(object sender, float speed)
    {
        UpdateButtons(speed);
    }

    private void UpdateButtons(float speed)
    {
        if(Mathf.Approximately(speed, 1.0f))
            speed1Button.AddToClassList("selectedSpeedButton");
        else
            speed1Button.RemoveFromClassList("selectedSpeedButton");

        if(Mathf.Approximately(speed, 2.0f))
            speed2Button.AddToClassList("selectedSpeedButton");
        else
            speed2Button.RemoveFromClassList("selectedSpeedButton");

        if(Mathf.Approximately(speed, 4.0f))
            speed3Button.AddToClassList("selectedSpeedButton");
        else
            speed3Button.RemoveFromClassList("selectedSpeedButton");

        if(Mathf.Approximately(speed, 8.0f))
            speed4Button.AddToClassList("selectedSpeedButton");
        else
            speed4Button.RemoveFromClassList("selectedSpeedButton");
    }
}
