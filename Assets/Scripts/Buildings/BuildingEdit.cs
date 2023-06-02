using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingEdit : MonoBehaviour
{
    public GameObject deleteWidget;
    public GameObject rotateWidget;
    public string widgetLayer = "Widgets";
    public Action BuildingDelete;
    private List<GameObject> allWidgets;
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = GameObject.Find("Main Camera").transform;

        allWidgets = new List<GameObject>() { deleteWidget, rotateWidget };
    }

    void Update()
    {
        AlwaysFaceTheCamera();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask(widgetLayer)))
        {
            foreach(var w in allWidgets)
            {
                var color = (w == hitInfo.transform.gameObject) ? Color.blue : Color.red;
                w.GetComponent<Renderer>().material.SetColor("_baseColor", color);
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (hitInfo.transform.gameObject == deleteWidget)
                    DeleteTarget();
                else if(hitInfo.transform.gameObject == rotateWidget)
                    RotateTarget();
            }
        }
        else
        {
            foreach(var w in allWidgets)
                w.GetComponent<Renderer>().material.SetColor("_baseColor", Color.red);
        }
    }

    private void DeleteTarget()
    {
        Debug.Log("Delete clicked");
        BuildingDelete?.Invoke();
    }

    private void RotateTarget()
    {
        transform.parent.Rotate(Vector3.up, 90);
        AlwaysFaceTheCamera();
    }

    private void AlwaysFaceTheCamera()
    {
        var n = cameraTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(n.x, 0, n.z));
    }
}
