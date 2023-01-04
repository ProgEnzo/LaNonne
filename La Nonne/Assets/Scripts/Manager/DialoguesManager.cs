using System;
using System.Collections.Generic;
using Core.Scripts.Utils;
using Shop;
using Unity.VisualScripting;
using UnityEngine;

namespace Manager
{
    public class DialoguesManager : MonoSingleton<DialoguesManager>
    {
        internal new static DialoguesManager instance;
        
        [SerializeField] private DialoguesSO dialoguesSo;

        internal List<string> currentTexts1 = new();
        internal List<string> currentTexts2 = new();
        internal List<string> currentTexts3 = new();

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            currentTexts1 = new List<string>(dialoguesSo.texts1);
            currentTexts2 = new List<string>(dialoguesSo.texts2);
            currentTexts3 = new List<string>(dialoguesSo.texts3);
        }
    }
}
