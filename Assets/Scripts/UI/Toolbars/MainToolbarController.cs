using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainToolbarController : MonoBehaviour
{
    UIDocument doc;

    void OnEnable()
    {
        doc = GetComponent<UIDocument>();

        doc.rootVisualElement.Q<Button>("buildToolbarButton").clicked += () => ToolbarController.Instance.ShowToolbar("BuildToolbar");

        doc.rootVisualElement.Q<Button>("mapButton").clicked += () => MenuController.Instance.ShowMenu("MapMenu");

        doc.rootVisualElement.Q<Button>("inventoryButton").clicked += () => MenuController.Instance.ShowMenu("InventoryMenu");

        doc.rootVisualElement.Q<Button>("jobsButton").clicked += () => MenuController.Instance.ShowMenu("JobsMenu");
        
        doc.rootVisualElement.Q<Button>("deliveryRequestsButton").clicked += () => MenuController.Instance.ShowMenu("DeliveryRequestsMenu");
        
        doc.rootVisualElement.Q<Button>("graphButton").clicked += () => MenuController.Instance.ShowMenu("GraphMenu");
    }
}
