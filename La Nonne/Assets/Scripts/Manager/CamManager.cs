using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Core.Scripts.Utils;
using UnityEngine;

namespace Camera
{
    public class CamManager : MonoSingleton<CamManager>
    {
        internal new static CamManager instance;
        [SerializeField] private List<CinemachineVirtualCamera> vCams = new List<CinemachineVirtualCamera>();
        [SerializeField] private Animator animator;
        private static readonly int BossCamState = Animator.StringToHash("bossCamState");
        private static readonly int PlayerCamState = Animator.StringToHash("playerCamState");
        private static readonly int PlayerDezoomCamState = Animator.StringToHash("playerDezoomCamState");

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
                //cam.LookAt = playerTransform;
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
        
        internal void ZoomDuringDash(int state)
        {
            animator.SetInteger(PlayerCamState, state);
        }

        internal void ChangeBossCamState(int state)
        {
            animator.SetInteger(BossCamState, state);
        }
    }
}
