using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ZOrderManager : MonoBehaviour
{

    public PlayerController player;
    public bool isPlayer;
    
        // Update is called once per frame
        void Update()
        {
            if (!isPlayer)
            {
                if (player.transform.position.y > transform.position.y)
                {
                    GetComponent<SpriteRenderer>().sortingOrder = -Mathf.FloorToInt(transform.position.y) + 20;
                }
                else
                {
                    GetComponent<SpriteRenderer>().sortingOrder = -Mathf.FloorToInt(transform.position.y);
                }
            }
            else
            {
                var i = 19;
                foreach (Transform x in player.GetComponentsInChildren<Transform>())
                {
                    if (x.GetComponent<SpriteRenderer>())
                    {
                        x.GetComponent<SpriteRenderer>().sortingOrder =
                            -Mathf.RoundToInt(player.transform.position.y) + i;
                        i--;
                    }
                }
            }
        }
}
