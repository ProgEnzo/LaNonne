using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col) //c'est debile mais c'est juste pour au moins le degager cet enfoiré
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
