using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Bleed", menuName = "ShopObject/Bleed")]
    public class BleedSO : ShopObjectSO
    {
        [SerializeField] internal int damage = 10;
        [SerializeField] internal float cooldown = 1f;
    }
}
