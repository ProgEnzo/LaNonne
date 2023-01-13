using Shop;
using Shop.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isSlotted;
    private int slotIndex;
    private Vector2 initialAnchoredPosition;
    private Vector2 initialLocalPosition;
    private Transform initialParent;
    [SerializeField] private int buttonNumber;
    private bool isDragging;
    internal static bool isAnyDragging;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        isSlotted = false;
        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialLocalPosition = rectTransform.localPosition;
        initialParent = transform.parent;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSlotted)
        {
            EffectManager.instance.appliedEffects[slotIndex] = EffectManager.Effect.None;
            rectTransform.anchoredPosition = initialAnchoredPosition;
            rectTransform.localPosition = initialLocalPosition;
            transform.SetParent(initialParent);
            isSlotted = false;
        }

        if (!isSlotted)
        {
            slotIndex = Drop.slotIndex;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (EffectManager.instance.effectInventory[(EffectManager.Effect)buttonNumber] > 0) // && !EffectManager.instance.appliedEffects.Contains((EffectManager.Effect)buttonNumber)
        {
            isDragging = true;
            isAnyDragging = true;
            WhipModifController.SelectEffect(buttonNumber);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            if ((!Drop.isPointerOnSlot || EffectManager.instance.appliedEffects[slotIndex] != EffectManager.Effect.None) && !isSlotted)
            {
                rectTransform.anchoredPosition = initialAnchoredPosition;
            }
            else if (!Drop.isPointerOnSlot && isSlotted)
            {
                EffectManager.instance.appliedEffects[slotIndex] = EffectManager.Effect.None;
                rectTransform.anchoredPosition = initialAnchoredPosition;
                rectTransform.localPosition = initialLocalPosition;
                transform.SetParent(initialParent);
                isSlotted = false;
            }
            else
            {
                slotIndex = Drop.slotIndex;
                isSlotted = true;
            }
            
            isDragging = false;
            isAnyDragging = false;
        }
    }
}
