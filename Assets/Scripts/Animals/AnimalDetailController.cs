using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalDetailController : MonoBehaviour
{
    public static AnimalDetailController Instance { get; private set; }

    public Transform TargetTransform { get; internal set; }

    public string animalLayer = "Creatures";
    public string uiLayer = "UI";
    public GameObject selectionCursorPrefab;

    private GameObject cursor;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void ClearSelection()
    {
        TargetTransform = null;
        Destroy(cursor);
        cursor = null;
        MenuController.Instance.Close("AnimalDetailsMenu");
    }

    void Update()
    {
        // No click, or click over UI
        if (!Mouse.current.leftButton.wasPressedThisFrame ||
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask(animalLayer, uiLayer)))
        {
            if (cursor != null)
            {
                Destroy(cursor);
                cursor = null;
            }

            TargetTransform = hit.transform;

            cursor = Instantiate(selectionCursorPrefab, TargetTransform);
            MenuController.Instance.ShowMenu("AnimalDetailsMenu");
        }
        else
        {
            if (cursor != null)
            {
                Destroy(cursor);
                cursor = null;
            }

            TargetTransform = null;
            MenuController.Instance.Close("AnimalDetailsMenu");
        }
    }
}
