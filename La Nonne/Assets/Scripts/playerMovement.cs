using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class playerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_rigidbody;
    
    public SO_Movement SO_Movement;
    
    [SerializeField] private float m_timerDash = 0f;

    public static playerMovement instance;

    private void Awake()
    {
        instance = this;
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetVelocity()
    {
        m_rigidbody.velocity = Vector2.zero;
    }

    private void Start()
    {
        ReInit();
    }
    
    public void ReInit()
    {
        transform.position = Vector3.zero;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_timerDash < -0.5f)
        {
            m_timerDash = SO_Movement.m_durationDash;
        }
        
        m_timerDash -= Time.deltaTime;

    }

    public void FixedUpdate()
    {
        m_rigidbody.drag = SO_Movement.dragDeceleration * SO_Movement.dragMultiplier;
        ManageMove();
    }

    private void ManageMove()
    {
        var speed = m_timerDash <= 0 ? SO_Movement.m_speed : SO_Movement.m_dashSpeed;

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
}
