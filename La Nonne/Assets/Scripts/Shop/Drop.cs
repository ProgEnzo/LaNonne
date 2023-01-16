using System.Collections;
using Shop;
using Shop.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    internal static bool isPointerOnSlot;
    internal static int slotIndex;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && EffectManager.instance.appliedEffects[slotIndex] == EffectManager.Effect.None && Drag.isAnyDragging)
        {
            StartCoroutine(DroppedCoroutine(eventData.pointerDrag));
        }
    }
    
    public static void PointerOnSlot(int buttonNumber)
    {
        isPointerOnSlot = true;
        slotIndex = buttonNumber;
    }
    
    public static void PointerOffSlot()
    {
        isPointerOnSlot = false;
    }
    
    private IEnumerator DroppedCoroutine(GameObject pointerDrag)
    {
        yield return new WaitWhile(() => Drag.isAnyDragging);
        pointerDrag.transform.SetParent(transform);
        pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        pointerDrag.GetComponent<RectTransform>().localPosition = Vector2.zero;
        WhipModificationController.AttachEffect(slotIndex);
    }
}
