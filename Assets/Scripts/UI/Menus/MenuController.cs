using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    private bool isVisible;
    private string activeMenu = "";

    private SoundPlayer soundPlayer;
    private SmolbeanInputActions inputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            DestroyImmediate(gameObject);
        else
            Instance = this;

        inputActions = new SmolbeanInputActions();
    }

    private void Start()
    {
        isVisible = true; // force this to true when the game starts to stop a sound playing :)
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        inputActions.Menus.Enable();

        ShowMenu();
    }

    private void OnEnable()
    {
        inputActions.Menus.ShowMenu.performed += OnShowMenu;
        inputActions.Menus.HideMenu.performed += OnHideMenu;
        inputActions.Menus.ToggleMenu.performed += OnToggleMenu;
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (!GameStateManager.Instance.IsStarted)
            return;

        if(isVisible)
            CloseAll();
        else
            ShowMenu();
    }

    private void OnHideMenu(InputAction.CallbackContext context)
    {
        // If the game hasn't started yet, don't close the menu!
        if (!GameStateManager.Instance.IsStarted)
            return;

        if (isVisible)
            CloseAll();
    }

    private void OnShowMenu(InputAction.CallbackContext context)
    {
        if (!GameStateManager.Instance.IsStarted)
            return;

        if(!isVisible)
            ShowMenu();
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

                if (isVisible == false)
                    soundPlayer.Play("Whoosh2");

                if(child.gameObject != null)
                    child.gameObject.SetActive(true);
            }
            else if(child.gameObject != null)
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
            child.gameObject.SetActive(false);

        activeMenu = "";
        isVisible = false;
        GameStateManager.Instance.Resume();
        ToolbarController.Instance.ShowToolbar();
    }

    public void Close(string name)
    {
        if (activeMenu == name)
            CloseAll();
    }
}
