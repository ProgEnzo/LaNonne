using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class WakeUpBoss : MonoBehaviour
{
    public BossStateManager boss;
    public AIDestinationSetter bossAIDestinationSetter;
    public Slider hpBossSlider;

    void Start()
    {
        boss = transform.parent.GetComponent<BossStateManager>();
        bossAIDestinationSetter = transform.parent.GetComponent<AIDestinationSetter>();
        hpBossSlider = GameObject.FindGameObjectWithTag("Boss HealthBar").GetComponent<Slider>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            boss.enabled = true;
            bossAIDestinationSetter.enabled = true;
            hpBossSlider.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
