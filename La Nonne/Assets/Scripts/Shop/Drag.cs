using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shop.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shop
{
    public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        internal RectTransform rectTransform;
        private Canvas canvas;
        internal bool isSlotted;
        internal int slotIndex;
        private Vector2 initialAnchoredPosition;
        internal Vector2 initialLocalPosition;
        internal Transform initialParent;
        private Transform previousParent;
        [SerializeField] private int buttonNumber;
        internal bool isActivatedFrame;
        private bool isDragging;
        internal static bool isAnyDragging;
        [SerializeField] private GameObject blackPanel;
        [SerializeField] internal Color emplacementColor;
        [SerializeField] private List<Image> emplacements;
        
        [Header("SoundEffect")] 
        public AudioSource whipAudioSource;
        public AudioClip whipClipSound;
        public AudioClip whipItemHoldSound;
        public AudioClip whipItemReleaseSound;
    
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

            if (isActivatedFrame)
            {
                CheckEmplacementColor();
                isActivatedFrame = false;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (EffectManager.instance.effectInventory[(EffectManager.Effect)buttonNumber] > 0) // && !EffectManager.instance.appliedEffects.Contains((EffectManager.Effect)buttonNumber)
            {
                //SOUND Play()
                whipAudioSource.PlayOneShot(whipClipSound);
                whipAudioSource.clip = whipItemHoldSound;
                whipAudioSource.Play();
                
                isDragging = true;
                isAnyDragging = true;
                previousParent = transform.parent;
                transform.SetParent(canvas.transform);
                GetComponent<Image>().raycastTarget = false;
                blackPanel.SetActive(true);
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
                //A CHAQUE FOIS QUON LACHE LA GEMME
                //SOUND DragSound.Stop() + PlayOneShot pour le drop
                whipAudioSource.Stop();
                StartCoroutine(WaitForSound());

                if ((!Drop.isPointerOnSlot || EffectManager.instance.appliedEffects[slotIndex] != EffectManager.Effect.None) && !isSlotted)
                {
                    transform.SetParent(previousParent);
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
                    if (slotIndex == exSlotIndex)
                    {
                        transform.SetParent(previousParent);
                        rectTransform.anchoredPosition = Vector2.zero;
                    }
                    else if (EffectManager.instance.appliedEffects[slotIndex] != EffectManager.Effect.None)
                    {
                        transform.SetParent(initialParent);
                        rectTransform.anchoredPosition = initialAnchoredPosition;
                        rectTransform.localPosition = initialLocalPosition;
                        isSlotted = false;
                    }
                }
                else
                {
                    isSlotted = true;
                }
            
                CheckEmplacementColor();
                GetComponent<Image>().raycastTarget = true;
                isDragging = false;
                isAnyDragging = false;
                blackPanel.SetActive(false);
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

        private IEnumerator WaitForSound()
        {
            whipAudioSource.PlayOneShot(whipItemReleaseSound);
            yield return new WaitForSeconds(whipItemReleaseSound.length);
                
            whipAudioSource.Stop();
        }
        
        private void CheckEmplacementColor()
        {
            if (isSlotted)
            {
                foreach (var emplacement in emplacements.Where(emplacement => emplacement.color == emplacementColor))
                {
                    emplacement.color = Color.white;
                }
                
                emplacements[slotIndex].color = emplacementColor;
            }
            else
            {
                foreach (var emplacement in emplacements.Where(emplacement => emplacement.color == emplacementColor))
                {
                    emplacement.color = Color.white;
                }
            }
        }
    }
}
