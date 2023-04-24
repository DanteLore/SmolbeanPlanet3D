using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCube : MonoBehaviour
{
    public GameObject highlightObject;

    void Start()
    {
        highlightObject.SetActive(false);
    }

    void OnMouseEnter()
    {
        highlightObject.SetActive(true);
    }

    void OnMouseExit()
    {
        highlightObject.SetActive(false);
    }
}
