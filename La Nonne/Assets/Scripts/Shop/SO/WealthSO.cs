using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Wealth", menuName = "ShopObject/Wealth")]
    public class WealthSO : ShopObjectSO
    {
        [Header("Wealth")]
        [SerializeField] internal float epDropRate = 1.5f;
        
        [Header("Profusion")]
        [SerializeField] internal int epExtraDrop = 5;
    }
}
