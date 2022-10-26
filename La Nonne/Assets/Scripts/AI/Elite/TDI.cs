using Controller;
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
    
        public void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 50)
            {
                Dissociate();
            }
        }

        private void Dissociate()
        {
            Destroy(gameObject);
            Instantiate(bully, transform.position, quaternion.identity);
            Instantiate(caretaker, transform.position, quaternion.identity);
        }
    }
}
