using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class lifeManager : MonoBehaviour
{

    public static lifeManager instance;
    
    [Header("playerLife")]
    [SerializeField] public float m_lifePoint = 3f;
    [SerializeField] public float m_currentLife;

    private void Start()
    {
        m_currentLife = m_lifePoint;
    }

    public void Awake()
    {
        if (instance == this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        Hit();
    }

    /*private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ennemy"))
        {
            m_currentLife -= 1;
        }
    }*/

    private void Hit()
    {
        if (m_currentLife <= 0)
        {
            Destroy(GameObject.FindWithTag("Player"));
        }
    }
}
