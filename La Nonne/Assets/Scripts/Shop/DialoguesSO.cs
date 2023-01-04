using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "DialoguesSO", menuName = "DialoguesSO")]
    public class DialoguesSO : ScriptableObject
    {
        [Header("Text")]
        [SerializeField] internal List<string> texts1;
        [SerializeField] internal List<string> texts2;
        [SerializeField] internal List<string> texts3;
    }
}
