using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class CircleBoxingManager : MonoBehaviour
{
    public int circleBoxingDamage;
    private void OnTriggerEnter2D(Collider2D col)
    {       
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(circleBoxingDamage);
        }
    }
}
