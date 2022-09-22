using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "playerController", menuName = "ScriptableObjects/new player Controller", order = 1)]

public class SO_Controller : ScriptableObject
{
    [Header("Mouvement")]
    public float m_speed;
    public float m_dashSpeed = 750f;
    public float m_durationDash = 0.35f;
    public float dragDeceleration = 12f;
    public float dragMultiplier = 12f;

    [Header("lifeManager")] 
    public int m_lifePoint = 3;
    public int m_currentLife;
}
