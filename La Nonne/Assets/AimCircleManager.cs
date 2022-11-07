using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class AimCircleManager : MonoBehaviour
{
    public int circleDamage = 20;
    private void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController.instance.TakeDamage(circleDamage);
    }
}
