using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHelp : MonoBehaviour
{
    public Transform launchPos;
    public GameObject epHelp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(epHelp, launchPos.position, transform.rotation);
        }
    }
}
