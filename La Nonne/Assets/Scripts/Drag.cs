using Shop;
using Shop.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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
            GetComponent<Image>().raycastTarget = false;
            WhipModificationController.SelectEffect(buttonNumber);
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
                transform.SetParent(initialParent);
                rectTransform.anchoredPosition = initialAnchoredPosition;
                rectTransform.localPosition = initialLocalPosition;
                isSlotted = false;
            }
            else if (isSlotted)
            {
                EffectManager.instance.appliedEffects[slotIndex] = EffectManager.Effect.None;
                var exSlotIndex = slotIndex;
                slotIndex = Drop.slotIndex;
                if (EffectManager.instance.appliedEffects[slotIndex] != EffectManager.Effect.None)
                {
                    transform.SetParent(initialParent);
                    rectTransform.anchoredPosition = initialAnchoredPosition;
                    rectTransform.localPosition = initialLocalPosition;
                    isSlotted = false;
                }
                else if (slotIndex == exSlotIndex)
                {
                    rectTransform.anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                isSlotted = true;
            }
            
            GetComponent<Image>().raycastTarget = true;
            isDragging = false;
            isAnyDragging = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && isSlotted)
        {
            EffectManager.instance.appliedEffects[slotIndex] = EffectManager.Effect.None;
            transform.SetParent(initialParent);
            rectTransform.anchoredPosition = initialAnchoredPosition;
            rectTransform.localPosition = initialLocalPosition;
            isSlotted = false;
        }
    }
}
