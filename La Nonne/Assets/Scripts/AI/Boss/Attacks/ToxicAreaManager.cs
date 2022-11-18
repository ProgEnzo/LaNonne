using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ToxicAreaManager : MonoBehaviour
{
    public int toxicAreaDamage;
    private void OnTriggerStay2D(Collider2D col)
    {       
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(toxicAreaDamage);
        }
    }
}
