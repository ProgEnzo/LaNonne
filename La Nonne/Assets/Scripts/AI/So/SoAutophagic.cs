using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "Autophagic", menuName = "ScriptableObjects/Autophagic", order = 1)]
    public class SoAutophagic : SO_Enemy
    {
        [Header("Detection")]
        [SerializeField] public float detectionRadius;
        
        [Header("Movement")]
        [SerializeField] public float avoidedAngle;
        [SerializeField] public float destinationMinRangeValue;
        [SerializeField] public float destinationMaxRangeValue;
        
        [Header("Auto Damage")]
        [SerializeField] public float autoDamagePart;
        [SerializeField] public float maxTimer;
        
        [Header("Stun Bar")]
        [SerializeField] public int stunBarMaxDamages;
        [SerializeField] public float stunBarDuration;
    }
}
