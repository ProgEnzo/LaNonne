using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Manager
{
    public class AnimationManager : MonoSingleton<AnimationManager>
    {
        internal new static AnimationManager instance;
        
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        
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
        
        internal void AnimationControllerInt(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int parameterToChange, int value)
        {
            if (parameterToChange  == DirectionState || parameterToChange == MovingState)
            {
                if (currentAnimPrefabAnimator.GetInteger(parameterToChange) != value)
                {
                    AnimationManagerInt(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, parameterToChange, value);
                    StartCoroutine(CanChangeCoroutine(currentAnimPrefabAnimator));
                }
            }
            else
            {
                if (!(currentAnimPrefabAnimator.GetInteger(DirectionState) == parameterToChange &&
                     currentAnimPrefabAnimator.GetInteger(MovingState) == value))
                {
                    AnimationManagerInt(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, parameterToChange, value);
                    StartCoroutine(CanChangeCoroutine(currentAnimPrefabAnimator));
                }
            }
        }

        private static IEnumerator CanChangeCoroutine(Animator currentAnimPrefabAnimator)
        {
            currentAnimPrefabAnimator.SetBool(CanChange, true);
            yield return new WaitForNextFrameUnit();
            currentAnimPrefabAnimator.SetBool(CanChange, false);
        }

        /*private IEnumerator AnimationControllerBool(int parameterToChange)
        {
            currentAnimPrefabAnimator.SetBool(CanChange, true);
            yield return new WaitForNextFrameUnit();
            currentAnimPrefabAnimator.SetBool(CanChange, false);
            currentAnimPrefabAnimator.SetBool(parameterToChange, true);
            yield return new WaitForNextFrameUnit();
            currentAnimPrefabAnimator.SetBool(parameterToChange, false);
        }*/

        private static void AnimationManagerInt(IReadOnlyList<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int parameterToChange, int value)
        {
            if (parameterToChange  == DirectionState)
            {
                AnimationManagerSwitch(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, value, currentAnimPrefabAnimator.GetInteger(MovingState),
                    currentAnimPrefabAnimator.GetBool(IsAttacking));
            }
            else if (parameterToChange == MovingState)
            {
                AnimationManagerSwitch(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, currentAnimPrefabAnimator.GetInteger(DirectionState), value,
                    currentAnimPrefabAnimator.GetBool(IsAttacking));
            }
            else
            {
                AnimationManagerSwitch(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, parameterToChange, value, 
                    currentAnimPrefabAnimator.GetBool(IsAttacking));
            }
            
            var currentAnimPrefabTemp = currentAnimPrefab;
            foreach (var prefab in animPrefabs.Where(prefab => prefab != currentAnimPrefabTemp))
            {
                prefab.SetActive(false);
            }
        }

        private static void AnimationManagerSwitch(IReadOnlyList<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int directionState, int movingState, bool isAttacking)
        {
            currentAnimPrefab = (directionState, movingState, isAttacking) switch
            {
                (0, 0, false) => animPrefabs[0],
                (0, 1, false) => animPrefabs[1],
                (0, >= 0, true) => animPrefabs[2],
                (1, 0, false) => animPrefabs[3],
                (1, 1, false) => animPrefabs[4],
                (1, >= 0, true) => animPrefabs[5],
                (2, 0, false) => animPrefabs[6],
                (2, 1, false) => animPrefabs[7],
                (2, >= 0, true) => animPrefabs[8],
                _ => animPrefabs[0]
            };

            currentAnimPrefab.SetActive(true);
            currentAnimPrefabAnimator = currentAnimPrefab.GetComponent<Animator>();
            
            currentAnimPrefabAnimator.SetInteger(DirectionState, directionState);
            currentAnimPrefabAnimator.SetInteger(MovingState, movingState);
            currentAnimPrefabAnimator.SetBool(IsAttacking, isAttacking);
        }
        
        internal void AnimationControllerBool(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int parameterToChange)
        {
            AnimationManagerBool(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, parameterToChange, true);
            StartCoroutine(CanChangeCoroutine(currentAnimPrefabAnimator));
        }
        
        internal void AnimationManagerBool(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int parameterToChange, bool value)
        {
            if (parameterToChange  == IsAttacking)
            {
                AnimationManagerSwitch(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, currentAnimPrefabAnimator.GetInteger(DirectionState), currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }

            var currentAnimPrefabTemp = currentAnimPrefab;
            foreach (var prefab in animPrefabs.Where(prefab => prefab != currentAnimPrefabTemp))
            {
                prefab.SetActive(false);
            }
        }

        internal void AnimationControllerBool(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int directionState, int parameterToChange)
        {
            AnimationManagerBool(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, directionState, parameterToChange, true);
            StartCoroutine(CanChangeCoroutine(currentAnimPrefabAnimator));
        }
        
        internal void AnimationManagerBool(List<GameObject> animPrefabs, ref GameObject currentAnimPrefab, ref Animator currentAnimPrefabAnimator, int directionState, int parameterToChange, bool value)
        {
            if (parameterToChange  == IsAttacking)
            {
                AnimationManagerSwitch(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, directionState, currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }

            var currentAnimPrefabTemp = currentAnimPrefab;
            foreach (var prefab in animPrefabs.Where(prefab => prefab != currentAnimPrefabTemp))
            {
                prefab.SetActive(false);
            }
        }
    }
}
