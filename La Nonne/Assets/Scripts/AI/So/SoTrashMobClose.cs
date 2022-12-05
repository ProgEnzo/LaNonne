using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "TrashMobClose", menuName = "ScriptableObjects/TrashMobClose", order = 1)]
    public class SoTrashMobClose : SO_Enemy
    {
        [Header("Knock-back")]
        [SerializeField] public float knockBackPower;
    }
}
