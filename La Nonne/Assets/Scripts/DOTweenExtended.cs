// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/07/13
// Extension Author: MZR
// Created : 2022/09/02

#if true && (UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_2017_1_OR_NEWER) // MODULE_MARKER

using System;
using Cinemachine;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

#pragma warning disable 1591
namespace DG.Tweening
{
    public static class DOTweenExtended
    {
        #region TextMeshPro
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOTMPScale(this TMP_Text target, float endValue, float duration)
        {
            Transform trans = target.transform;
            Vector3 endValueV3 = new Vector3(endValue, endValue, endValue);
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => trans.localScale, x => trans.localScale = x, endValueV3, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<string, string, StringOptions> DOTMPText(this TMP_Text target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            TweenerCore<string, string, StringOptions> t = DOTween.To(() => target.text, x => target.text = x, endValue, duration);
            t.SetOptions(richTextEnabled, scrambleMode, scrambleChars)
                .SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOTMPFontSize(this TMP_Text target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.fontSize, x => target.fontSize = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOCharacterSpacing(this TMP_Text target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.characterSpacing, x => target.characterSpacing = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region Variables
        public static TweenerCore<float, float, FloatOptions> DOFloat(this float target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target, x => target = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region Tilemap
        public static TweenerCore<Color, Color, ColorOptions> DOTilemapFade(this Tilemap target, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<Color, Color, ColorOptions> DOTilemapColor(this Tilemap target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        
        #endregion
        
        #region Light2D
        public static TweenerCore<float, float, FloatOptions> DOLight2DIntensity(this Light2D target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.intensity, x => target.intensity = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<Color, Color, ColorOptions> DOLight2DColor(this Light2D target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOLight2DInnerAngle(this Light2D target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.pointLightInnerAngle, x => target.pointLightInnerAngle = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOLight2DInnerRadius(this Light2D target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.pointLightInnerRadius, x => target.pointLightInnerRadius = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOLight2DOuterAngle(this Light2D target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.pointLightOuterAngle, x => target.pointLightOuterAngle = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOLight2DOuterRadius(this Light2D target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.pointLightOuterRadius, x => target.pointLightOuterRadius = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region BoxCollider2D
        public static TweenerCore<Vector2, Vector2, VectorOptions> DOBoxCollider2DSize(this BoxCollider2D target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.size, x => target.size = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<Vector2, Vector2, VectorOptions> DOBoxCollider2DOffset(this BoxCollider2D target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.offset, x => target.offset = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region CircleCollider2D
        public static TweenerCore<float, float, FloatOptions> DOCircleCollider2DRadius(this CircleCollider2D target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.radius, x => target.radius = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<Vector2, Vector2, VectorOptions> DOCircleCollider2DOffset(this CircleCollider2D target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(() => target.offset, x => target.offset = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region Particles
        public static TweenerCore<float, float, FloatOptions> DOParticlesSimulationSpeed(this ParticleSystem target, float endValue, float duration)
        {
            var targetMain = target.main;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => targetMain.simulationSpeed, x => targetMain.simulationSpeed = x, endValue, duration);
            t.SetTarget(target.main);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOParticleGravity(this ParticleSystem target, float endValue, float duration)
        {
            var targetMain = target.main;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => targetMain.gravityModifierMultiplier, x => targetMain.gravityModifierMultiplier = x, endValue, duration);
            t.SetTarget(targetMain);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOParticleSpeed(this ParticleSystem target, float endValue, float duration)
        {
            var targetMain = target.main;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => targetMain.startSpeedMultiplier, x => targetMain.startSpeedMultiplier = x, endValue, duration);
            t.SetTarget(targetMain);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOParticleEmission(this ParticleSystem target, float endValue, float duration)
        {
            var targetMain = target.emission;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => targetMain.rateOverTimeMultiplier, x => targetMain.rateOverTimeMultiplier= x, endValue, duration);
            t.SetTarget(targetMain);
            return t;
        }
        #endregion
        
        #region Cinemachine
        public static TweenerCore<float, float, FloatOptions> DOCinemachineOrthoSize(this CinemachineVirtualCamera target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.m_Lens.OrthographicSize, x => target.m_Lens.OrthographicSize = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region Canvas
        public static TweenerCore<float, float, FloatOptions> DOCanvasScaleFactor(this CanvasScaler target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.scaleFactor, x => target.scaleFactor = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        
        public static TweenerCore<Vector3, Vector3, VectorOptions> DORectTransformMove(this RectTransform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.position, x => target.position = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        
        #endregion
        
        #region PostProcess
        public static TweenerCore<float, float, FloatOptions> DOVignetteIntensity(this Volume target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.GetComponent<Vignette>().intensity.value, x => target.GetComponent<Vignette>().intensity.value = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOVignetteSmoothness(this Volume target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.GetComponent<Vignette>().smoothness.value, x => target.GetComponent<Vignette>().smoothness.value = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<Color, Color, ColorOptions> DOVignetteColor(this Volume target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.GetComponent<Vignette>().color.value, x => target.GetComponent<Vignette>().color.value = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOBloomIntensity(this Volume target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.GetComponent<Bloom>().intensity.value, x => target.GetComponent<Bloom>().intensity.value = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOChromaticAberrationIntensity(this Volume target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.GetComponent<ChromaticAberration>().intensity.value, x => target.GetComponent<ChromaticAberration>().intensity.value = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOVolumeWeight(this Volume target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.weight, x => target.weight= x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
        
        #region TrailRenderer
        public static TweenerCore<Color, Color, ColorOptions> DOTrailRendererFadeBegin(this TrailRenderer target, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => target.startColor, x => target.startColor = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<Color, Color, ColorOptions> DOTrailRendererFadeEnd(this TrailRenderer target, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => target.endColor, x => target.endColor = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        public static TweenerCore<float, float, FloatOptions> DOTrailRendererLenght(this TrailRenderer target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.time, x => target.time = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        #endregion
    }
}
#endif
