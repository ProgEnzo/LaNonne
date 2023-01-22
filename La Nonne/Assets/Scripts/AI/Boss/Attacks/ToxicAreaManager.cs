using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ToxicAreaManager : MonoBehaviour
{
    public int toxicAreaDamage;
    public float timeBetweenDamage;
    private void OnTriggerStay2D(Collider2D col)
    {       
        timeBetweenDamage += Time.deltaTime;

        if (col.gameObject.CompareTag("Player") && timeBetweenDamage >= 1f)
        {
            PlayerController.instance.TakeDamage(toxicAreaDamage);
            timeBetweenDamage = 0f;

        }
    }
}
