using Core.Scripts.Utils;
using UnityEngine;

namespace Manager
{
    public class InputManager : MonoSingleton<InputManager>
    {
        internal new static InputManager instance;
        
        [Header("Move Inputs")]
        [SerializeField] internal KeyCode leftMoveKey;
        [SerializeField] internal KeyCode rightMoveKey;
        [SerializeField] internal KeyCode upMoveKey;
        [SerializeField] internal KeyCode downMoveKey;
        
        [Header("Dash Inputs")]
        [SerializeField] internal KeyCode dashKey;
        [SerializeField] internal KeyCode slowMoKey;
        
        [Header("Attack Inputs")]
        [SerializeField] internal KeyCode zealousBladeKey;
        [SerializeField] internal KeyCode inquisitorialChainKey;
        [SerializeField] internal KeyCode revealingDashKey;
        
        [Header("Map Inputs")]
        [SerializeField] internal KeyCode mapKey;
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
    }
}
