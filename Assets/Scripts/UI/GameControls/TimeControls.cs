using UnityEngine;
using UnityEngine.UIElements;

public class GameSpeedControls : MonoBehaviour
{
    private Button speed1Button;
    private Button speed2Button;
    private Button speed3Button;
    private Button speed4Button;
    private Label timeLabel;
    private Label dayLabel;

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

        timeLabel = document.rootVisualElement.Q<Label>("timeLabel");

        UpdateButtons(GameStateManager.Instance.SelectedGameSpeed);
        GameStateManager.Instance.GameSpeedChanged += GameSpeedChanged;
    }

    void Update()
    {
        timeLabel.text = DayNightCycleController.Instance.DisplayTimeAndDay;
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
