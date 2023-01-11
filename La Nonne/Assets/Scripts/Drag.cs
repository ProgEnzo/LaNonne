using System;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Camera camera;
    [SerializeField] private static List<GameObject> gems;

    private void Start()
    {
        camera = Camera.main;
        camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private static void ClickOnEffect(int effect)
    {
        Instantiate(gems[effect]);
    }
    
    private static void ClickOnEmplacement(int emplacement)
    {
        Destroy(gems[emplacement]);
    }
}
