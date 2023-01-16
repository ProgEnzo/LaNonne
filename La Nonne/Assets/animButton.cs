using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class animButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float animScaleMultiplier;
    private Vector3 defaultScale;
    private RectTransform _rectTransform;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        defaultScale = _rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.DOScale(defaultScale*animScaleMultiplier, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.DOScale(defaultScale, 0.2f);
    }
}
