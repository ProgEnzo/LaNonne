using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Core.Scripts.Utils;
using UnityEngine;

public class ShakeCamera : MonoSingleton<ShakeCamera>
{
    public static ShakeCamera camInstance;

    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;
    
    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public static void Shake(float timeOfShake)
    {
        camInstance.StopAllCoroutines();
        camInstance.StartCoroutine(camInstance.DoTheShake(timeOfShake));
    }

    public IEnumerator DoTheShake(float timeOfShake)
    {
        while (timeOfShake > 0)
        {
            noise.m_AmplitudeGain = 1f;
            noise.m_FrequencyGain = 1f;
            timeOfShake -= Time.deltaTime;
            yield return null;
        }
        
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }
}
