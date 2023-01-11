using System;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
