using System;
using Shop;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isSlotted;
    private int slotIndex;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        isSlotted = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSlotted)
        {
            EffectManager.instance.appliedEffects[slotIndex] = EffectManager.Effect.None;
            Destroy(gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!Drop.isPointerOnSlot)
        {
            Destroy(gameObject);
        }
        else
        {
            isSlotted = true;
            slotIndex = Drop.slotIndex;
        }
    }
}
