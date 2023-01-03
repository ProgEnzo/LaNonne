using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealingDrop : MonoBehaviour
{
    private PlayerController playerController;
    private CircleCollider2D circleCollider2D;

    private void Start()
    {
        playerController = PlayerController.instance;
        circleCollider2D = GetComponent<CircleCollider2D>();

        
        StartCoroutine(WaitTrigger());

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerController.HealPlayer(20);
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitTrigger()
    {
        circleCollider2D.enabled = false;
        yield return new WaitForSeconds(0.5f);
        circleCollider2D.enabled = true;

    }
}
