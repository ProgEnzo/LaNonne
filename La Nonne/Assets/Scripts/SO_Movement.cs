using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "playerMovement", menuName = "ScriptableObjects/new player Movement", order = 1)]

public class SO_Movement : ScriptableObject
{
    [Header("Mouvement")]
    public float m_speed;
    public float m_dashSpeed = 750f;
    public float m_durationDash = 0.35f;
    public float dragDeceleration = 12f;
    public float dragMultiplier = 12f;
}
