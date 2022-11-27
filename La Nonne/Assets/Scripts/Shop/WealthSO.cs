using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Wealth", menuName = "ShopObject/Wealth")]
    public class WealthSO : ShopObjectSO
    {
        [SerializeField] internal float epDropRate = 1.5f;
    }
}
