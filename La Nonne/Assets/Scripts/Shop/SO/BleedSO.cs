using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Bleed", menuName = "ShopObject/Bleed")]
    public class BleedSO : ShopObjectSO
    {
        [Header("Bleed")]
        [SerializeField] internal int damage = 10;
        [SerializeField] internal float cooldown = 1f;
        
        [Header("Vampirism")]
        [SerializeField] internal float healPart = 1f;
    }
}
