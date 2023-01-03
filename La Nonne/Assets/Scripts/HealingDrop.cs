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

    private GameObject healDropObject;
    public float magnetSpeed;
    public bool isMagnet;
    private void Start()
    {
        playerController = PlayerController.instance;
        circleCollider2D = GetComponent<CircleCollider2D>();

        circleCollider2D.enabled = false;

        StartCoroutine(WaitTrigger());

    }

    private void Update()
    {
        if (isMagnet)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerController.transform.position, magnetSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerController.HealPlayer(20);
            Destroy(gameObject);
            isMagnet = false;
        }
    }

    private IEnumerator WaitTrigger()
    {
        transform.DOMove(new Vector2(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f)), 0.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.5f);

        circleCollider2D.enabled = true;
        isMagnet = true;
        //transform.DOMove(playerController.transform.position, 0.5f).SetEase(Ease.InQuad);
    }
}
