using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public string defaultMenuName = "MainMenu";

    public static MenuController Instance { get; private set; }

    private bool isVisible;
    private string activeMenu = "";

    private Dictionary<KeyCode, string> hotkeyLookup;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        hotkeyLookup = gameObject
                            .GetComponentsInChildren<SmolbeanMenu>(true)
                            .Where(m => m.hotKey != KeyCode.None)
                            .ToDictionary(m => m.hotKey, m => m.name);
    }

    void Start()
    {
        ShowMenu();
    }

    void Update()
    {
        // No hotkeys when the game hasn't started yet!
        if(!GameStateManager.Instance.IsStarted)
            return;

        // Escape always closes the current menu
        if(isVisible && Input.GetKeyDown(KeyCode.Escape))
        {    
            CloseAll();
        }
        else
        {        
            // Check hotkeys
            foreach(KeyCode key in hotkeyLookup.Keys)
            {
                if(Input.GetKeyDown(key))
                {
                    if(!isVisible)
                        ShowMenu(hotkeyLookup[key]);
                    else if(hotkeyLookup[key] == activeMenu)
                        CloseAll();

                    break; // Take the first pressed hotkey
                }
            }
        }
    }


    public void ShowMenu(string menuName = "MainMenu")
    {
        ToolbarController.Instance.CloseAll();

        foreach(var child in gameObject.GetComponentsInChildren<SmolbeanMenu>(true))
        {
            if(child.name == menuName)
            {
                if(child.shouldPauseGame)
                    GameStateManager.Instance.Pause();

                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        activeMenu = menuName;
        isVisible = true;
    }

    public void CloseAll()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        activeMenu = "";
        isVisible = false;
        GameStateManager.Instance.Resume();
        ToolbarController.Instance.ShowToolbar();
    }
}
