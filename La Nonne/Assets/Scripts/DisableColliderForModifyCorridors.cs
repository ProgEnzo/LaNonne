using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class DisableColliderForModifyCorridors : MonoBehaviour
{
    private CapsuleCollider2D capsCol;
    private PlayerController player;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            capsCol = player.GetComponent<CapsuleCollider2D>();
            capsCol.enabled = false;
        }
    }
}
