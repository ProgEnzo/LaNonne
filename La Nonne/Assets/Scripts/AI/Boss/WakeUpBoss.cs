using System;
using System.Collections;
using System.Collections.Generic;
using AI.Boss;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class WakeUpBoss : MonoBehaviour
{
    public BossStateManager boss;
    public Animator bossAnimator;
    public AIDestinationSetter bossAIDestinationSetter;
    public GameObject healthBarBoss;
    private Image hpBossBarImage;

    void Start()
    {
        boss = transform.parent.GetComponent<BossStateManager>();
        bossAIDestinationSetter = transform.parent.GetComponent<AIDestinationSetter>();
        healthBarBoss = GameObject.FindWithTag("Boss HealthBar");
        healthBarBoss.SetActive(false);
        bossAnimator.enabled = false;

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            bossAnimator.enabled = true; //desac animator parce que le son se joue tous le temps à cause des animations events

            healthBarBoss.SetActive(true);
            boss.enabled = true;
            bossAIDestinationSetter.enabled = true;
            Destroy(gameObject);
        }
    }
}
