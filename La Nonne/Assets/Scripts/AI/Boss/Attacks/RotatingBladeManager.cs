using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class RotatingBladeManager : MonoBehaviour
{
    public int rotatingBladeDamage;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(rotatingBladeDamage);
        }
    }
}
