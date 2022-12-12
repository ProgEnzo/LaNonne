using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GoToBoss : MonoBehaviour
{
    private RoomContentGenerator roomContentGenerator;

    private void Update()
    {
        VaLaBas();
    }

    private void VaLaBas()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //transform.DOMove(new Vector3(roomContentGenerator.mapBoss, roomContentGenerator.mapBoss,0))
            transform.position = Vector2.MoveTowards(transform.position, roomContentGenerator.mapBoss, 4f);
        }
    }
}
