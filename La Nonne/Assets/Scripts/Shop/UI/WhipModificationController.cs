using System.Collections;
using System.Collections.Generic;
using Controller;
using Manager;
using TMPro;
using UnityEngine;

namespace Shop.UI
{
   public class WhipModificationController : MonoBehaviour
   {
      [Header("References")]
      [SerializeField] private GameObject whipModificationMenu;
      public GameObject shopCanvas;

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

         uiManager = UIManager.instance;
         isWhipModificationMenuOpened = false;

         for (var i = 1; i < 5; i++)
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
            shopCanvas.SetActive(false);

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
         if (Input.GetKeyDown(InputManager.instance.interactKey) && !uiManager.IsAnyMenuOpened() && !PlayerController.instance.isRevealingDashOn && !PlayerController.instance.chainBlade.isWarningOn)
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
               effectVariableTextComponents[3][i].text = "";
            }
            else
            {
               effectVariableTextComponents[0][i].text = "Applied effect : " + EffectManager.instance.effectDictionary[i][EffectManager.instance.effectInventory[(EffectManager.Effect)i]-1].effectDescription;
               effectVariableTextComponents[1][i].text = "Imploded effect : " + EffectManager.instance.effectDictionary[i][EffectManager.instance.effectInventory[(EffectManager.Effect)i]-1].superEffectDescription;
               effectVariableTextComponents[2][i].text = "Level : " + EffectManager.instance.effectInventory[(EffectManager.Effect)i];
               effectVariableTextComponents[3][i].text = "Application probability : " + EffectManager.instance.effectDictionary[i][EffectManager.instance.effectInventory[(EffectManager.Effect)i]-1].chanceToBeApplied + "%";
            }
         }
      }
      
      private void PlaceGems()
      {
         for (var i = 0; i < EffectManager.instance.effectInventory.Count; i++)
         {
            var gem = gems[i];
            var dragScript = gem.GetComponent<Drag>();

            dragScript.isActivatedFrame = true;
            
            if (EffectManager.instance.effectInventory[(EffectManager.Effect)i] == 0)
            {
               gem.SetActive(false);
            }
            else
            {
               gem.SetActive(true);
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
               gem.transform.SetParent(whipModificationMenu.transform.GetChild(i + 4));
               dragScript.rectTransform.localPosition = Vector3.zero;
               dragScript.isSlotted = true;
               dragScript.slotIndex = i;
            }
         }
      }
   }
}
