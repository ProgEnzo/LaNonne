using System.Collections;
using System.Collections.Generic;
using AI.Trash;
using UnityEngine;

public class WakeUpTrashMobRange : MonoBehaviour
{
    public TrashMobRange trashMobRange;
    void Start()
    {
        trashMobRange = transform.parent.parent.GetComponent<TrashMobRange>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            trashMobRange.enabled = true;
            Destroy(gameObject);
        }
    }
}
