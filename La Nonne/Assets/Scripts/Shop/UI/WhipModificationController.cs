using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.UI
{
   public class WhipModificationController : MonoBehaviour
   {
      [Header("References")]
      [SerializeField] private GameObject whipModificationMenu;
      public GameObject shopCanvas;
      public Image image;

      [SerializeField] private float timeToAccess;

      private float timerInputPressed;
      private bool isInTrigger;

      private UIManager uiManager;
      private bool isWhipModificationMenuOpened;
      private static EffectManager.Effect _selectedEffect;
      public UiAnimWhipModifMenu uiAnimWhipModifMenu;
      [SerializeField] private AnimationClip whipModifMenuAnimClip;
      
      private readonly List<List<TextMeshProUGUI>> effectVariableTextComponents = new();

      [SerializeField] private List<GameObject> gems;

      private void Start()
      {
         StartCoroutine(BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp());
         shopCanvas.SetActive(false);

         //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!

         timerInputPressed = 0f;
         uiManager = UIManager.instance;
         isWhipModificationMenuOpened = false;

         for (var i = 1; i < 4; i++)
         {
            effectVariableTextComponents.Add(new List<TextMeshProUGUI>());
            for (var j = 0; j < EffectManager.instance.effectInventory.Count; j++)
            {
               effectVariableTextComponents[i-1].Add(whipModificationMenu.transform.GetChild(1).GetChild(j).GetChild(i).GetComponent<TextMeshProUGUI>());
            }
         }

         //Texte des effets
         for (var i = 0; i < EffectManager.instance.effectInventory.Count; i++)
         {
            whipModificationMenu.transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((EffectManager.Effect)i).ToString();
         }
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
         
         if (Input.GetKeyDown(InputManager.instance.quitKey) && isWhipModificationMenuOpened)
         {
            CloseWhipModificationMenu();
         }
      }

      private void OpenWhipModificationMenu()
      {
         if (Input.GetKey(InputManager.instance.interactKey) && !uiManager.IsAnyMenuOpened() && !PlayerController.instance.isRevealingDashOn && !PlayerController.instance.chainBlade.isWarningOn)
         {
            timerInputPressed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(0, 1, timerInputPressed / timeToAccess);

            if (timerInputPressed > timeToAccess - 0.05f && !isWhipModificationMenuOpened)
            {
               isWhipModificationMenuOpened = true;
               uiManager.isWhipMenuOpened = true;
               
               uiManager.DesactivateInGameUI();
               
               whipModificationMenu.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
               uiAnimWhipModifMenu.OpenMenu();
               Time.timeScale = 0;

               ChangeEffectTexts();
               PlaceGems();
            }
         }

         if (Input.GetKeyUp(InputManager.instance.interactKey))
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
         isWhipModificationMenuOpened = false;
         StartCoroutine(DisableIsWhipMenuOpenedCoroutine());
         uiAnimWhipModifMenu.CloseMenu();
         StartCoroutine(DisableWhipMenuOpenedCoroutine());
         
         uiManager.ActivateInGameUI();

         image.fillAmount = 0f;

         Time.timeScale = 1;
      }
      
      public static void SelectEffect(int buttonNumber)
      {
         _selectedEffect = (EffectManager.Effect)buttonNumber;
      }

      public static void AttachEffect(int slotIndex)
      {
         EffectManager.instance.appliedEffects[slotIndex] = _selectedEffect;
         //whipModificationMenu.transform.GetChild(selectedEffectEmplacement+3).GetChild(0).GetComponent<TextMeshProUGUI>().text = EffectManager.instance.appliedEffects[selectedEffectEmplacement] + "\n\n" + EffectManager.instance.effectInventory[EffectManager.instance.appliedEffects[selectedEffectEmplacement]];
      }

      private IEnumerator DisableWhipMenuOpenedCoroutine()
      {
         yield return new WaitForSecondsRealtime(whipModifMenuAnimClip.length);
         uiManager.isWhipMenuOpened = false;
      }

      private void ChangeEffectTexts()
      {
         for (var i = 0; i < EffectManager.instance.effectInventory.Count; i++)
         {
            if (EffectManager.instance.effectInventory[(EffectManager.Effect)i] == 0)
            {
               effectVariableTextComponents[0][i].text = "";
               effectVariableTextComponents[1][i].text = "Not acquired.";
               effectVariableTextComponents[2][i].text = "";
            }
            else
            {
               effectVariableTextComponents[0][i].text = EffectManager.instance.effectDictionary[i][EffectManager.instance.effectInventory[(EffectManager.Effect)i]-1].superEffectDescription;
               effectVariableTextComponents[1][i].text = "Level : " + EffectManager.instance.effectInventory[(EffectManager.Effect)i];
               effectVariableTextComponents[2][i].text = "Probability : " + EffectManager.instance.effectDictionary[i][EffectManager.instance.effectInventory[(EffectManager.Effect)i]-1].chanceToBeApplied + "%";
            }
         }
      }
      
      private void PlaceGems()
      {
         for (var i = 0; i < EffectManager.instance.effectInventory.Count; i++)
         {
            var gem = gems[i];
            if (EffectManager.instance.effectInventory[(EffectManager.Effect)i] == 0)
            {
               gem.SetActive(false);
            }
            else
            {
               gem.SetActive(true);
               var dragScript = gem.GetComponent<Drag>();
               gem.transform.SetParent(dragScript.initialParent);
               dragScript.rectTransform.localPosition = dragScript.initialLocalPosition;
               dragScript.isSlotted = false;
            }
         }
         
         for (var i = 0; i < EffectManager.instance.appliedEffects.Length; i++)
         {
            if (EffectManager.instance.appliedEffects[i] != EffectManager.Effect.None)
            {
               var gem = gems[(int)EffectManager.instance.appliedEffects[i]];
               var dragScript = gem.GetComponent<Drag>();
               gem.transform.SetParent(whipModificationMenu.transform.GetChild(i + 3));
               dragScript.rectTransform.localPosition = Vector3.zero;
               dragScript.isSlotted = true;
               dragScript.slotIndex = i;
            }
         }
      }
   }
}
