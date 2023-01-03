using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "DialoguesSO", menuName = "DialoguesSO")]
    public class DialoguesSO : ScriptableObject
    {
        [Header("Text")]
        [SerializeField] internal List<string> text;
    }
}
