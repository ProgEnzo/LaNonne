using System.Collections;
using Shop.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shop
{
    public class Drop : MonoBehaviour, IDropHandler
    {
        internal static bool isPointerOnSlot;
        internal static int slotIndex;
    
        [Header("SoundEffect")] 
        public AudioSource whipAudioSource;
        public AudioClip whipClipSound;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null && EffectManager.instance.appliedEffects[slotIndex] == EffectManager.Effect.None && Drag.isAnyDragging)
            {
                //SOUND ON DROP sur le whipchain
                whipAudioSource.PlayOneShot(whipClipSound);
            
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
}
