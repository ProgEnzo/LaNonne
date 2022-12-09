using UnityEngine;

namespace Controller
{
    [CreateAssetMenu(fileName = "playerController", menuName = "ScriptableObjects/new player Controller", order = 1)]

    public class SO_Controller : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] public float moveSpeed;
        [SerializeField] public float dashSpeed;
        [SerializeField] public float durationDash;
        [SerializeField] public float dragDeceleration;
        [SerializeField] public float dragMultiplier;
    
        [Header("Health")]
        [SerializeField] public int maxHealth;

        [Header("Zealous Blade")]
        [SerializeField] public int zealousBladeDamage;
        [SerializeField] public float zealousBladeHitLength;
        [SerializeField] public float zealousBladeHitAngle;
        [SerializeField] public float zealousBladeHitSpeed;
        [SerializeField] public float zealousBladeToleranceAngle;
        [SerializeField] public int zealousBladeMaxHitState;
        [SerializeField] public float zealousBladeMaxDuringComboCooldown;
        [SerializeField] public float zealousBladeMaxNextComboCooldown;
        [SerializeField] public float zealousBladeMaxDetectionAngle;
        [SerializeField] public float zealousBladeLittleHitStopDuration;
        [SerializeField] public float zealousBladeLittleKnockBackForce;
        [SerializeField] public float zealousBladeBigHitStopDuration;
        [SerializeField] public float zealousBladeBigKnockBackForce;

        [Header("Inquisitorial Chain")]
        [SerializeField] public int inquisitorialChainDamage;
        [SerializeField] public float inquisitorialChainChainHitLength;
        [SerializeField] public float inquisitorialChainBladeHitLength;
        [SerializeField] public float inquisitorialChainHitAngle;
        [SerializeField] public float inquisitorialChainHitSpeed;
        [SerializeField] public float inquisitorialChainToleranceAngle;
        [SerializeField] public float inquisitorialChainCooldownTime;

        [Header("Revealing Dash")]
        [SerializeField] public int revealingDashDamage;
        [SerializeField] public float revealingDashStunDuration;
        [SerializeField] public float revealingDashToleranceDistance;
        [SerializeField] public float revealingDashHitSpeed;
        [SerializeField] public float revealingDashDetectionRadius;
        [SerializeField] public float revealingDashSlowTimeSpeed;
        
        [Header("Slow Motion")]
        [SerializeField] public float slowMoCooldown;
        [SerializeField] public float slowMoDuration;
        [SerializeField] public float slowMoSpeed;
        [SerializeField] public float slowMoPlayerSpeedFactor;
    }
}
