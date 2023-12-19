using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarController : MonoBehaviour
{
    public string defaultToolbarName = "MainToolbar";

    public static ToolbarController Instance { get; private set; }

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
    
    public void ShowToolbar(string menuName = null)
    {
        if(menuName == null)
            menuName = defaultToolbarName;

        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(child.gameObject.name == menuName);
        }
    }

    public void CloseAll()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
