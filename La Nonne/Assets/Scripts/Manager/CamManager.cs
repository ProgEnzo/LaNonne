using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cinemachine;
using Controller;
using Core.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

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
        private static readonly int PlayerDashState = Animator.StringToHash("playerDashState");
        
        [Header("Scriptable Object")]
        [SerializeField][FormerlySerializedAs("SO_Controller")] internal SO_Controller soController;
        
        private static readonly int ChainBladeState = Animator.StringToHash("ChainBladeState");
        private static readonly int BladeState = Animator.StringToHash("chainBladeState");

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
        
        internal void PlayerDashStateChange()
        {
            StartCoroutine(PlayerIsDashingCamEffect());
        }

        private IEnumerator PlayerIsDashingCamEffect()
        {
            animator.SetInteger(PlayerDashState, 1);
            yield return new WaitForSeconds(0.35f);
            animator.SetInteger(PlayerDashState, 0);
        }

        internal void ChainBladeCamDezoomState(int state)
        {
            animator.SetInteger(BladeState, state);
        }

        internal void ChainBladeCamShakeState()
        {
            StartCoroutine(ChainBladeCd());
        }

        private IEnumerator ChainBladeCd()
        {
            animator.SetInteger(BladeState, 2);
            yield return new WaitForSeconds(0.3f);
            animator.SetInteger(BladeState, 0);
        }
    }
}
