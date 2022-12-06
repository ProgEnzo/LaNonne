using System;
using System.Collections;
using System.Collections.Generic;
using AI.Boss;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class WakeUpBoss : MonoBehaviour
{
    public BossStateManager boss;
    public AIDestinationSetter bossAIDestinationSetter;
    public GameObject healthBarBoss;

    void Start()
    {
        boss = transform.parent.GetComponent<BossStateManager>();
        bossAIDestinationSetter = transform.parent.GetComponent<AIDestinationSetter>();
        healthBarBoss = GameObject.FindWithTag("Boss HealthBar");
        healthBarBoss.SetActive(false);

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            boss.enabled = true;
            bossAIDestinationSetter.enabled = true;
            healthBarBoss.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
