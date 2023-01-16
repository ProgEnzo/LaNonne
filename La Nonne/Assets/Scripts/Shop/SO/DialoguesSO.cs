using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "DialoguesSO", menuName = "DialoguesSO")]
    public class DialoguesSO : ScriptableObject
    {
        [Header("Text")]
        [SerializeField] [TextArea] internal List<string> texts1;
        [SerializeField] [TextArea] internal List<string> texts2;
        [SerializeField] [TextArea] internal List<string> texts3;
        [SerializeField] [TextArea] internal List<string> texts4;
    }
}
