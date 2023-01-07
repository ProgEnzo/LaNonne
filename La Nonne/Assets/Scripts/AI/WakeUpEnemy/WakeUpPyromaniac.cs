using System.Collections;
using System.Collections.Generic;
using AI.Elite;
using Pathfinding;
using UnityEngine;

public class WakeUpPyromaniac : MonoBehaviour
{
    public Pyromaniac pyromaniac;
    void Start()
    {
        pyromaniac = transform.parent.parent.GetComponent<Pyromaniac>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            pyromaniac.enabled = true;
            Destroy(gameObject);
        }
    }
}
