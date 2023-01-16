using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GenPro.Rooms.Generator;
using UnityEngine;

public class GoToBoss : MonoBehaviour
{
    private RoomContentGenerator roomContentGenerator;
    public Rigidbody2D rb;

    public float speedForBouboule = 5f;

    public void OnEnable()
    {
        roomContentGenerator = GameObject.Find("RoomContentGenerator").GetComponent<RoomContentGenerator>();
    }
    
    private void Update()
    {
        VaLaBas();
    }

    private void VaLaBas()
    {
        rb.velocity = (roomContentGenerator.mapBoss - (Vector2) transform.position).normalized * speedForBouboule;
    }
}
