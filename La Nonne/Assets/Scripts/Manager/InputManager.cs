using Core.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        [Header("Attack Inputs")]
        [SerializeField] internal KeyCode zealousBladeKey;
        [SerializeField] internal KeyCode inquisitorialChainKey;
        [SerializeField] internal KeyCode revealingDashKey;
        
        [Header("Other Inputs")]
        [SerializeField] internal KeyCode pauseKey;
        [SerializeField] internal KeyCode quitKey;
        [SerializeField] internal KeyCode interactKey;
        [FormerlySerializedAs("goToBossKey")] [SerializeField] internal KeyCode helpKey;

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
