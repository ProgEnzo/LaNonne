using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    internal static bool isPointerOnSlot;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
    
    internal static void PointerOnSlot()
    {
        isPointerOnSlot = true;
    }
    
    internal static void PointerOffSlot()
    {
        isPointerOnSlot = false;
    }
}
