using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "TrashMobRange", menuName = "ScriptableObjects/TrashMobRange", order = 1)]
    public class SoTrashMobRange : SO_Enemy
    {
        [Header("Range Values")]
        [SerializeField] public float maxProxRange;
        [SerializeField] public float shootingRange;
        [SerializeField] public float aggroRange;
        
        [Header("Shoot Values")]
        [SerializeField] public float cooldownBetweenShots;
        [SerializeField] public float bulletSpeed;
        [SerializeField] public int bulletDamage;
        
        [Header("Knock-back")]
        [SerializeField] public float knockBackBody;
    }
}
