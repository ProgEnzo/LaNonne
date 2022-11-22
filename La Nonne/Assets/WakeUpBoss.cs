using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class WakeUpBoss : MonoBehaviour
{
    public BossStateManager boss;
    public AIDestinationSetter bossAIDestinationSetter;
    void Start()
    {
        boss = transform.parent.GetComponent<BossStateManager>();
        bossAIDestinationSetter = transform.parent.GetComponent<AIDestinationSetter>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            boss.enabled = true;
            bossAIDestinationSetter.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
