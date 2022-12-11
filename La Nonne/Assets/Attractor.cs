using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public float attractorSpeed = 5f;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            transform.position = Vector2.MoveTowards(transform.position, col.transform.position, attractorSpeed * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (transform.childCount < 1)
        {
            Destroy(gameObject);
        }
    }
}