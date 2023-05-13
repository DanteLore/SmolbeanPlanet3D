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

        CloseAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isVisible)
            {
                CloseAll();
            }
            else
            {
                ShowMenu();
            }
        }
    }


    public void ShowMenu(string menuName = "MainMenu")
    {
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
    }
}
