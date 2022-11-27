using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Target", menuName = "ShopObject/Target")]
    public class TargetSO : ShopObjectSO
    {
        [SerializeField] internal float rateOfDamage = 1.5f;
    }
}
