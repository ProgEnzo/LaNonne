using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "Bully", menuName = "ScriptableObjects/Bully", order = 1)]
    public class SoBully : SO_Enemy
    {
        [Header("Knock-back")]
        [SerializeField] public float knockBackPower;
    }
}
