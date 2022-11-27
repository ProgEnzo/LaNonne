using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Chill", menuName = "ShopObject/Chill")]
    public class ChillSO : ShopObjectSO
    {
        [SerializeField] internal float rateOfBasicSpeed = 0.7f;
    }
}
