using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class DashMineManager : MonoBehaviour
{
    public int dashMineDamage;
    private void OnTriggerStay2D(Collider2D col)
    {       
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(dashMineDamage);
        }
    }
}
