using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "Caretaker", menuName = "ScriptableObjects/Caretaker", order = 1)]
    public class SoCaretaker : SO_Enemy
    {
        [Header("Circle Values")]
        [SerializeField] public int circleDamage;
        [SerializeField] public int healAmount;
        [SerializeField] public float timeBetweenCircleSpawn;
        [SerializeField] public float attackRange;
        
        [Header("Knock-back")]
        [SerializeField] public float bodyKnockBack;
    }
}
