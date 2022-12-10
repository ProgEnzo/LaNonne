using System.Collections;
using System.Linq;
using Controller;
using Core.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Manager
{
    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        internal new static AnimationManager instance;

        private PlayerController playerController;
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int AttackState = Animator.StringToHash("attackState");
        
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
            playerController = PlayerController.instance;
        }

        #region Player

        internal void AnimationControllerPlayer(int parameterToChange, int value)
        {
            if (parameterToChange == DirectionState || parameterToChange == MovingState || parameterToChange == AttackState)
            {
                if (playerController.currentAnimPrefabAnimator.GetInteger(parameterToChange) != value)
                {
                    if (parameterToChange == AttackState && value != 0)
                    {
                        AnimationManagerPlayer(MovingState, 0);
                    }
                    AnimationManagerPlayer(parameterToChange, value);
                }
            }
            else
            {
                if (playerController.currentAnimPrefabAnimator.GetInteger(DirectionState) != parameterToChange || playerController.currentAnimPrefabAnimator.GetInteger(MovingState) != value)
                {
                    AnimationManagerPlayer(DirectionState, parameterToChange);
                    AnimationManagerPlayer(MovingState, value);
                }
            }
        }

        private void AnimationManagerPlayer(int parameterToChange, int value)
        {
            if (parameterToChange == DirectionState)
            {
                AnimationManagerSwitchPlayer(value, playerController.currentAnimPrefabAnimator.GetInteger(MovingState),
                    playerController.currentAnimPrefabAnimator.GetInteger(AttackState));
            }
            else if (parameterToChange == MovingState)
            {
                AnimationManagerSwitchPlayer(playerController.currentAnimPrefabAnimator.GetInteger(DirectionState), value,
                    playerController.currentAnimPrefabAnimator.GetInteger(AttackState));
            }
            else if (parameterToChange == AttackState)
            {
                AnimationManagerSwitchPlayer(playerController.currentAnimPrefabAnimator.GetInteger(DirectionState),
                    playerController.currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }
            
            var currentAnimPrefabTemp = playerController.currentAnimPrefab;
            foreach (var prefab in playerController.animPrefabs.Where(prefab => prefab != currentAnimPrefabTemp))
            {
                prefab.SetActive(false);
            }
        }

        #endregion

        private void AnimationManagerSwitchPlayer(int directionState, int movingState, int attackState)
        {
            playerController.currentAnimPrefab = (directionState, movingState, attackState) switch
            {
                (0, 1, 0) => playerController.animPrefabs[0],
                (0, 2, 0) => playerController.animPrefabs[1],
                (1, 1, 0) => playerController.animPrefabs[2],
                (1, 2, 0) => playerController.animPrefabs[3],
                (2, 1, 0) => playerController.animPrefabs[4],
                (2, 2, 0) => playerController.animPrefabs[5],
                (>= 0, 3, 0) => playerController.animPrefabs[6],
                (>= 0, >= 0, >= 1) => playerController.animPrefabs[7],
                _ => playerController.animPrefabs[0]
            };

            playerController.currentAnimPrefab.SetActive(true);
            playerController.currentAnimPrefabAnimator = playerController.currentAnimPrefab.GetComponent<Animator>();
            
            playerController.currentAnimPrefabAnimator.SetInteger(DirectionState, directionState);
            playerController.currentAnimPrefabAnimator.SetInteger(MovingState, movingState);
            playerController.currentAnimPrefabAnimator.SetInteger(AttackState, attackState);
        }
    }
}
