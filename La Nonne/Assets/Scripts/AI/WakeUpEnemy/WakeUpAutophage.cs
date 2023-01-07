using System.Collections;
using System.Collections.Generic;
using AI.Elite;
using Pathfinding;
using UnityEngine;

public class WakeUpAutophage : MonoBehaviour
{
    public Autophagic autophage;
    void Start()
    {
        autophage = transform.parent.parent.GetComponent<Autophagic>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            autophage.enabled = true;
            Destroy(gameObject);
        }
    }
}
