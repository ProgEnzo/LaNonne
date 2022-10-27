using System;
using System.Collections;
using Controller;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace AI.Elite
{
    public class TDI : MonoBehaviour
    {
        [Header("Enemy Health")] 
        [SerializeField] public float currentHealth;
        [SerializeField] public float maxHealth;
        
        [Header("Components")] 
        [SerializeField] public GameObject bully;
        [SerializeField] public GameObject caretaker;
        public PlayerController playerController;
        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Awake()
        {
            //Assignation du script au prefab ON SPAWN
            playerController = PlayerController.instance;
        }

        public void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 50)
            {
                StartCoroutine(DieAndSpawn());
            }
        }
        
        IEnumerator DieAndSpawn()
        {
            transform.DOScale(new Vector3(3, 0, 3), 0.1f);
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
            Instantiate(bully, transform.position, quaternion.identity);
            Instantiate(caretaker, transform.position, quaternion.identity);
            
        }
    }
}
