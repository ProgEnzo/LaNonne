using Shop.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    internal static bool isPointerOnSlot;
    internal static int slotIndex;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            WhipModifController.AttachEffect(slotIndex);
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
}
