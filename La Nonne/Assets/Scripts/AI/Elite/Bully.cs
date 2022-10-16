using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bully : MonoBehaviour
{
    [Header("Enemy Health")] 
    [SerializeField] public float currentHealth;
    [SerializeField] public float maxHealth;
    
    [Header("Enemy Attack")]
    [SerializeField] private int bullyDamage;
    [SerializeField] private float knockbackPower;

    [Header("Enemy Components")]
    public playerController playerController;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    #region HealthEnemyClose
    public void TakeDamageFromPlayer(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            BullyDie();
        }
    }
    
    private void BullyDie()
    {
        Destroy(gameObject);
    }
    
    #endregion
    

    private void OnCollisionEnter2D(Collision2D col) 
    {
        //Si le bully touche le player
        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlayerIsHit());
            playerController.TakeDamage(bullyDamage); //Player takes damage

            Collider2D collider2D = col.collider; //the incoming collider2D (celle du player en l'occurence)
            Vector2 direction = (collider2D.transform.position - transform.position).normalized;
            Vector2 knockback = direction * knockbackPower;
            
            playerController.m_rigidbody.AddForce(knockback, ForceMode2D.Impulse);
        }
    }


    IEnumerator PlayerIsHit()
    {
        playerController.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        playerController.GetComponent<SpriteRenderer>().color = Color.yellow;

    }
}
