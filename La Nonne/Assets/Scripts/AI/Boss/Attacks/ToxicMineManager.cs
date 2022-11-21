using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ToxicMineManager : MonoBehaviour
{
    public int toxicMineDamage;

    private void Start()
    {
        StartCoroutine(DealsDamage());
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {       
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(toxicMineDamage);
        }
    }

    private IEnumerator DealsDamage()
    {
        
        yield return new WaitForSeconds(1f);
    }
}
