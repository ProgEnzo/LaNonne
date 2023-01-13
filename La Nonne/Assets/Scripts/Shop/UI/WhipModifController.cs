using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.UI
{
   public class WhipModifController : MonoBehaviour
   {
      [Header("References")]
      [SerializeField] private GameObject whipModificationMenu;
      public GameObject shopCanvas;
      public Image image;
   
      [SerializeField] private float timeToAccess;

      private float timerInputPressed;
      private bool isInTrigger;

      private UIManager uiManager;
      private EffectManager effectManager;
      private bool isWhipModifOpened;
      private static EffectManager.Effect _selectedEffect;
      
      [SerializeField] private List<GameObject> gems;

      private void Start()
      {
         StartCoroutine(BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp());
         shopCanvas.SetActive(false);

         //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!

         timerInputPressed = 0f;
         effectManager = EffectManager.instance;
         uiManager = UIManager.instance;
         isWhipModifOpened = false;
      }

      private IEnumerator BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp()
      {
         yield return new WaitForSeconds(0.3f);
         whipModificationMenu.SetActive(false);
      }

      private void OnTriggerStay2D(Collider2D col)
      {
         if (col.gameObject.CompareTag("Player"))
         {
            shopCanvas.SetActive(true);

            isInTrigger = true;
         }
      }

      private void OnTriggerExit2D(Collider2D other)
      {
         if (other.gameObject.CompareTag("Player"))
         {
            timerInputPressed = 0f;
            shopCanvas.SetActive(false);

            image.fillAmount = 0f;

            isInTrigger = false;
         }
      }

      private void Update()
      {
         if (shopCanvas == true && isInTrigger)
         {
            OpenWhipModificationMenu();
         }
         
         if (Input.GetKeyDown(KeyCode.Escape) && isWhipModifOpened)
         {
            CloseWhipModificationMenu();
         }
      }

      private void OpenWhipModificationMenu()
      {
         if (Input.GetKey(KeyCode.E) && !uiManager.isGamePaused && !uiManager.isShopOpened && !uiManager.isWhipMenuOpened)
         {
            timerInputPressed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(0, 1, timerInputPressed / timeToAccess);

            if (timerInputPressed > timeToAccess - 0.05f && !isWhipModifOpened)
            {
               isWhipModifOpened = true;
               uiManager.isWhipMenuOpened = true;
               
               uiManager.DesactivateInGameUI();
               
               whipModificationMenu.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
               Time.timeScale = 0;
               
               for (var i = 0; i < EffectManager.instance.effectInventory.Count; i++)
               {
                  whipModificationMenu.transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((EffectManager.Effect)i).ToString(); // + "\n\n" + EffectManager.instance.effectInventory[(EffectManager.Effect)i];
               }

               for (var i = 0; i < EffectManager.instance.appliedEffects.Length; i++)
               {
                  if (EffectManager.instance.appliedEffects[i] != EffectManager.Effect.None)
                  {
                     whipModificationMenu.transform.GetChild(i+3).GetChild(0).GetComponent<TextMeshProUGUI>().text = EffectManager.instance.appliedEffects[i] + "\n\n" + EffectManager.instance.effectInventory[EffectManager.instance.appliedEffects[i]];
                  }
               }
            }
         }

         if (Input.GetKeyUp(KeyCode.E))
         {
            timerInputPressed = 0f;
            image.DOFillAmount(0,0.5f);
         }
      }
      
      private IEnumerator DisableIsWhipMenuOpenedCoroutine()
      {
         yield return new WaitForSecondsRealtime(0.01f);
         uiManager.isWhipMenuOpened = false;
      }

      public void CloseWhipModificationMenu()
      {
         isWhipModifOpened = false;
         StartCoroutine(DisableIsWhipMenuOpenedCoroutine());
         whipModificationMenu.SetActive(false);
         
         uiManager.ActivateInGameUI();

         image.fillAmount = 0f;

         Time.timeScale = 1;
      }
      
      public void SelectEffect(int buttonNumber)
      {
         if (EffectManager.instance.effectInventory[(EffectManager.Effect)buttonNumber] > 0 && !effectManager.appliedEffects.Contains((EffectManager.Effect)buttonNumber))
         {
            _selectedEffect = (EffectManager.Effect)buttonNumber;
         }
      }

      public static void AttachEffect(int slotIndex)
      {
         EffectManager.instance.appliedEffects[slotIndex] = _selectedEffect;
         //whipModificationMenu.transform.GetChild(selectedEffectEmplacement+3).GetChild(0).GetComponent<TextMeshProUGUI>().text = EffectManager.instance.appliedEffects[selectedEffectEmplacement] + "\n\n" + EffectManager.instance.effectInventory[EffectManager.instance.appliedEffects[selectedEffectEmplacement]];
      }
   }
}
