using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Chill", menuName = "ShopObject/Chill")]
    public class ChillSO : ShopObjectSO
    {
        [Header("Chill")]
        [SerializeField] internal float rateOfBasicSpeed = 0.7f;
        
        [Header("Freeze")]
        [SerializeField] internal float freezeTime = 1f;
    }
}
