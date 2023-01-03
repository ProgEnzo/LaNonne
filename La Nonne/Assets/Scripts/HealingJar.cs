using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealingJar : MonoBehaviour
{
    public GameObject healDrop;
    private GameObject healDropObject;
    public int numberOfHealDrops;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < numberOfHealDrops; i++)
                {
                    healDropObject = Instantiate(healDrop, transform.position, Quaternion.identity);
                    healDropObject.transform.DOMove(new Vector2(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f)), 0.5f).SetEase(Ease.OutQuad);
                }
                Destroy(gameObject);

            }
        }
    }
}
