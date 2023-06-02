using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public string defaultMenuName = "MainMenu";

    public static MenuController Instance { get; private set; }

    private bool isVisible;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        CloseAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isVisible)
                CloseAll();
            else
                ShowMenu();
        }
        else if (Input.GetKeyDown(KeyCode.M) && !isVisible)
        {
            ShowMenu("MapMenu");
        }
    }


    public void ShowMenu(string menuName = "MainMenu")
    {
        GameStateManager.Instance.Pause();

        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(child.gameObject.name == menuName);
        }

        isVisible = true;
    }

    public void CloseAll()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        isVisible = false;
        GameStateManager.Instance.Resume();
    }
}
