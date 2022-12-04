using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "TDI", menuName = "ScriptableObjects/TDI", order = 1)]
    public class SoTdi : SO_Enemy
    {
        [Header("Circle Values")]
        [SerializeField] public int circleDamage;
        [SerializeField] public float timeBetweenCircleSpawn;
        [SerializeField] public float healAmount;
        
        [Header("Knock-back")]
        [SerializeField] public int bodyKnockBack;
    }
}
