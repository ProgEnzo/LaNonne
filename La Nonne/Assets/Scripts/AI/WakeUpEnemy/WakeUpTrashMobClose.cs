using System.Collections;
using System.Collections.Generic;
using AI.Trash;
using UnityEngine;

public class WakeUpTrashMobClose : MonoBehaviour
{
    public TrashMobClose trashMobClose;
    void Start()
    {
        trashMobClose = transform.parent.parent.GetComponent<TrashMobClose>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            trashMobClose.enabled = true;
            Destroy(gameObject);
        }
    }
}
