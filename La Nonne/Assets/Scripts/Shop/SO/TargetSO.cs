using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Target", menuName = "ShopObject/Target")]
    public class TargetSO : ShopObjectSO
    {
        [Header("Target")]
        [SerializeField] internal float rateOfDamage = 1.5f;
        
        [Header("Burst")]
        [SerializeField] internal int burstDamage = 50;
    }
}
