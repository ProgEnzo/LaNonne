using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Elite
{
    public class Pyromane : MonoBehaviour
    {
        [Header("Enemy Health")] [SerializeField]
        public float currentHealth;

        [Header("Enemy Attack")] [SerializeField]
        private int circleDamage;

        [SerializeField] private float knockbackPower;
        [SerializeField] private float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;

        [Header("Enemy Components")] public PlayerController playerController;
        public SO_Controller soController;
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleDamageSprite;
        [SerializeField] private GameObject circleHealSprite;

        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        public bool isStunned;

        private void Update()
        {
            //if 
        }
    }
}