using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Utils;
using UnityEngine;

namespace Manager
{
    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        internal new static AnimationManager instance;

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

        #region Player

        internal void AnimationControllerPlayer(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int parameterToChange, int value)
        {
            if (parameterToChange == DirectionState || parameterToChange == MovingState || parameterToChange == AttackState)
            {
                if (currentAnimPrefabAnimator.GetInteger(parameterToChange) != value)
                {
                    if (parameterToChange == AttackState && value != 0)
                    {
                        AnimationManagerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, MovingState, 0);
                    }
                    AnimationManagerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, parameterToChange, value);
                }
            }
            else
            {
                if (currentAnimPrefabAnimator.GetInteger(DirectionState) != parameterToChange || currentAnimPrefabAnimator.GetInteger(MovingState) != value)
                {
                    AnimationManagerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, DirectionState, parameterToChange);
                    AnimationManagerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, MovingState, value);
                }
            }
        }

        private void AnimationManagerPlayer(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int parameterToChange, int value)
        {
            if (parameterToChange == DirectionState)
            {
                AnimationManagerSwitchPlayer(animPrefabs, out currentAnimPrefab, out currentAnimPrefabAnimator, value, currentAnimPrefabAnimator.GetInteger(MovingState),
                    currentAnimPrefabAnimator.GetInteger(AttackState));
            }
            else if (parameterToChange == MovingState)
            {
                AnimationManagerSwitchPlayer(animPrefabs, out currentAnimPrefab, out currentAnimPrefabAnimator, currentAnimPrefabAnimator.GetInteger(DirectionState), value,
                    currentAnimPrefabAnimator.GetInteger(AttackState));
            }
            else if (parameterToChange == AttackState)
            {
                AnimationManagerSwitchPlayer(animPrefabs, out currentAnimPrefab, out currentAnimPrefabAnimator, currentAnimPrefabAnimator.GetInteger(DirectionState),
                    currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }
            
            var currentAnimPrefabTemp = currentAnimPrefab;
            foreach (var prefab in animPrefabs.Where(prefab => prefab != currentAnimPrefabTemp))
            {
                prefab.SetActive(false);
            }
        }

        #endregion

        private void AnimationManagerSwitchPlayer(List<GameObject> animPrefabs, out GameObject currentAnimPrefab, out Animator currentAnimPrefabAnimator, int directionState, int movingState, int attackState)
        {
            currentAnimPrefab = (directionState, movingState, attackState) switch
            {
                (0, 1, 0) => animPrefabs[0],
                (0, 2, 0) => animPrefabs[1],
                (1, 1, 0) => animPrefabs[2],
                (1, 2, 0) => animPrefabs[3],
                (2, 1, 0) => animPrefabs[4],
                (2, 2, 0) => animPrefabs[5],
                (>= 0, 3, 0) => animPrefabs[6],
                (>= 0, >= 0, >= 1) => animPrefabs[7],
                _ => animPrefabs[0]
            };

            currentAnimPrefab.SetActive(true);
            currentAnimPrefabAnimator = currentAnimPrefab.GetComponent<Animator>();
            
            currentAnimPrefabAnimator.SetInteger(DirectionState, directionState);
            currentAnimPrefabAnimator.SetInteger(MovingState, movingState);
            currentAnimPrefabAnimator.SetInteger(AttackState, attackState);
        }
    }
}
