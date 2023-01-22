using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class DashMineManager : MonoBehaviour
{
    public int dashMineDamage;
    public float timeBetweenDamage;
    private void OnTriggerStay2D(Collider2D col)
    {
        timeBetweenDamage += Time.deltaTime;

        if (col.gameObject.CompareTag("Player") && timeBetweenDamage >= 1f)
        {
            PlayerController.instance.TakeDamage(dashMineDamage);
            timeBetweenDamage = 0f;

        }
       
    }
}
