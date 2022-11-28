using System.Collections;
using System.Collections.Generic;
using AI.Elite;
using UnityEngine;

public class WakeUpTDI : MonoBehaviour
{
    public TDI tdi;
    void Start()
    {
        tdi = transform.parent.GetComponent<TDI>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            tdi.enabled = true;
            Destroy(gameObject);
        }
    }
}
