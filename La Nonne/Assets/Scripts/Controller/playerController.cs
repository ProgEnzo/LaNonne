using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class playerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_rigidbody;
    
    public SO_Controller SO_Controller;
    
    [SerializeField] private float m_timerDash = 0f;

    public static playerController instance;

    public RoomFirstDungeonGenerator rfg;

    private void Start()
    {
        SO_Controller.currentHealth = SO_Controller.maxHealth;
        Debug.Log(" CURRENT HEALTH AT START : " + SO_Controller.currentHealth);
        ReInit();
    }

    private void Awake()
    {
        instance = this;
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetVelocity()
    {
        m_rigidbody.velocity = Vector2.zero;
    }

    public void ReInit()
    {
        transform.position = new Vector3(rfg.roomCenters[0].x, rfg.roomCenters[0].y, 0);
    }
    
    void OnDestroy()
    {
        instance = null;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_timerDash < -0.5f)
        {
            m_timerDash = SO_Controller.m_durationDash;
        }
        
        m_timerDash -= Time.deltaTime;

    }

    public void FixedUpdate()
    {
        m_rigidbody.drag = SO_Controller.dragDeceleration * SO_Controller.dragMultiplier;
        ManageMove();
    }

    private void ManageMove()
    {
        var speed = m_timerDash <= 0 ? SO_Controller.m_speed : SO_Controller.m_dashSpeed;

        int nbInputs = (Input.GetKey(KeyCode.Z) ? 1 : 0) + (Input.GetKey(KeyCode.Q) ? 1 : 0) +
                       (Input.GetKey(KeyCode.S) ? 1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        if (nbInputs > 1) speed *= 0.75f;

        if (Input.GetKey(KeyCode.Z))
        {
            m_rigidbody.AddForce(Vector2.up*speed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            m_rigidbody.AddForce(Vector2.left*speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            m_rigidbody.AddForce(Vector2.down*speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            m_rigidbody.AddForce(Vector2.right*speed);
        }
    }
    
    public void TakeDamage(int damage)
    {
        
        SO_Controller.currentHealth -= damage;
        Debug.Log("PLAYER HAS BEEN HIT, HEALTH REMAINING : " + SO_Controller.currentHealth);

        if (SO_Controller.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Destroy(gameObject);
        Debug.Log("PLAYER IS NOW DEAD");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  
}
