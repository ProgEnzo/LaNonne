using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cinemachine;
using Core.Scripts.Utils;
using UnityEngine;

namespace Manager
{
    public class CamManager : MonoSingleton<CamManager>
    {
        internal new static CamManager instance;
        [SerializeField] private List<CinemachineVirtualCamera> vCams = new List<CinemachineVirtualCamera>();
        [SerializeField] private Animator animator;
        private static readonly int BossCamState = Animator.StringToHash("bossCamState");
        private static readonly int PlayerCamState = Animator.StringToHash("playerCamState");
        private static readonly int PlayerDezoomCamState = Animator.StringToHash("playerDezoomCamState");
        private static readonly int PlayerRevealingDashState = Animator.StringToHash("playerRevealingDashState");
        private static readonly int BocalDestroy = Animator.StringToHash("bocalDestroy");
        private static readonly int PlayerHasBeenHit = Animator.StringToHash("playerHasBeenHit");

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
        
        public void FocusCameraOnThePlayer(Transform playerTransform) 
        {
            foreach (var cam in vCams)
            {
                cam.Follow = playerTransform;
            }
        }

        internal IEnumerator ShakeCam()
        {
            //DO SHAKE CAMERA
            animator.SetInteger(PlayerCamState, 1);
            yield return new WaitForSeconds(0.3f);
            animator.SetInteger(PlayerCamState, 0);
        }

        internal void DezoomDuringCombo(int state)
        {
            animator.SetInteger(PlayerDezoomCamState, state);
        }
        
        internal void ZoomDuringRevealingDash(int state)
        {
            animator.SetInteger(PlayerRevealingDashState, state);
        }

        internal void ChangeBossCamState(int state)
        {
            animator.SetInteger(BossCamState, state);
        }

        internal void DestroyingHealthJarState(int state)
        {
            if (state == 1)
            {
                StartCoroutine(DestroyHealthJar());
            }
            else
            {
                animator.SetInteger(BocalDestroy, state);
            }
        }

        internal void PlayerHasBeenHitByEnnemy()
        {
            StartCoroutine(playerHasBeenHitCd());
        }

        private IEnumerator DestroyHealthJar()
        {
            animator.SetInteger(BocalDestroy, 1);
            yield return new WaitForSeconds(0.2f);
            animator.SetInteger(BocalDestroy, 0);
        }

        private IEnumerator playerHasBeenHitCd()
        {
            animator.SetInteger(PlayerHasBeenHit, 1);
            yield return new WaitForSeconds(0.1f);
            animator.SetInteger(PlayerHasBeenHit, 0);
        }
    }
}
