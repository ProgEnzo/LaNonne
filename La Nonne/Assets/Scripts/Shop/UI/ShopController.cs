using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Shop.UI
{
   public class ShopController : MonoBehaviour
   {
      [Header("References")] 
      [SerializeField] private GameObject shopPanel;
      [SerializeField] private GameObject whipModificationMenu;
      public GameObject shopCanvas;
      public Image image;
   
      [SerializeField] private float timeToAccess;

      private float timerInputPressed;
      private bool isInTrigger;

      [SerializeField] private int numberOfOfferedObjects;
      private EffectManager.Effect[] effectsInTheShop;
      private UIManager uiManager;
      private EffectManager effectManager;
      private bool isShopOpened;
      private bool hasShopBeenOpened;
      private bool canChooseEffect;
      private int selectedEffectEmplacement;
      private bool isEffectEmplacementSelected;
      [SerializeField] private int maxNumberOfTakenObjects;
      private int currentNumberOfTakenObjects;

      private void Start()
      {
         StartCoroutine(BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp());
         shopCanvas.SetActive(false);

         //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!

         timerInputPressed = 0f;
         effectManager = EffectManager.instance;
         uiManager = UIManager.instance;
         hasShopBeenOpened = false;
         isShopOpened = false;
         canChooseEffect = false;
         isEffectEmplacementSelected = false;
      }

      private IEnumerator BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp()
      {
         yield return new WaitForSeconds(0.3f);
         shopPanel.SetActive(false);
         whipModificationMenu.SetActive(false);
      }

      private void OnTriggerStay2D(Collider2D col)
      {
         if (col.gameObject.CompareTag("Player"))
         {
            shopCanvas.SetActive(true);
         }

         isInTrigger = true;
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
            OpenShop();
         }
      }

      private void OpenShop()
      {
         if (Input.GetKey(KeyCode.E))
         {
            timerInputPressed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(0, 1, timerInputPressed / timeToAccess);

            if (timerInputPressed > timeToAccess - 0.05f && !isShopOpened)
            {
               isShopOpened = true;
               
               uiManager.DesactivateInGameUI();
               
               shopPanel.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
               Time.timeScale = 0;
               if (!hasShopBeenOpened)
               {
                  hasShopBeenOpened = true;
                  currentNumberOfTakenObjects = 0;
                  ShopObjectsSelector();
               }
               for (var i = 0; i < effectsInTheShop.Length; i++)
               {
                  if (effectsInTheShop[i] != EffectManager.Effect.None)
                  {
                     /*shopPanel.transform.GetChild(i + 1).GetChild(0).GetComponent<Image>().sprite =
                        "Cost : " + effectManager.effectDictionary[(int)effectsInTheShop[i]][
                           EffectManager.instance.effectInventory[effectsInTheShop[i]]].image;*/
                     shopPanel.transform.GetChild(i + 1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        effectsInTheShop[i] + " - T" +
                        (EffectManager.instance.effectInventory[effectsInTheShop[i]] + 1);
                     shopPanel.transform.GetChild(i + 1).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                        "Cost : " + effectManager.effectDictionary[(int)effectsInTheShop[i]][
                           EffectManager.instance.effectInventory[effectsInTheShop[i]]].cost;
                     shopPanel.transform.GetChild(i + 1).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                        effectManager.effectDictionary[(int)effectsInTheShop[i]][
                           EffectManager.instance.effectInventory[effectsInTheShop[i]]].description;
                  }
                  else
                  {
                     for (var j = 0; j < 3; j++)
                     {
                        shopPanel.transform.GetChild(i + 1).GetChild(j).GetComponent<TextMeshProUGUI>().text = "Closed.";
                     }
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

      private void ShopObjectsSelector()
      {
         effectsInTheShop = new EffectManager.Effect[numberOfOfferedObjects];
         for (var i = 0; i < effectsInTheShop.Length; i++)
         {
            effectsInTheShop[i] = EffectManager.Effect.None;
         }

         var possibleEffects = new List<EffectManager.Effect>();
         
         for (var i = 0; i < effectManager.numberOfEffects; i++)
         {
            if (EffectManager.instance.effectInventory[(EffectManager.Effect)i] < effectManager.effectMaxLevel)
            {
               possibleEffects.Add((EffectManager.Effect)i);
            }
         }
         
         var maxPossibleEffects = possibleEffects.Count;

         for (var i = 0; i < Math.Min(effectsInTheShop.Length, maxPossibleEffects); i++)
         {
            effectsInTheShop[i] = possibleEffects[Random.Range(0, possibleEffects.Count)];
            possibleEffects.Remove(effectsInTheShop[i]);
         }
      }

      public void CloseShop()
      {
         isShopOpened = false;
         shopPanel.SetActive(false);
         
         uiManager.ActivateInGameUI();

         image.fillAmount = 0f;

         if (currentNumberOfTakenObjects > 0)
         {
            Destroy(gameObject);
         }

         Time.timeScale = 1;
      }
   
      public void CloseWhipModificationMenu()
      {
         isEffectEmplacementSelected = false;
         whipModificationMenu.SetActive(false);
      
         //shopPanel.SetActive(true);
      }

      public void OpenWhipModificationMenu(GameObject menu)
      {
         menu.SetActive(true);
         for (var i = 0; i < EffectManager.instance.effectInventory.Count; i++)
         {
            menu.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = (EffectManager.Effect)i + "\n\n" + EffectManager.instance.effectInventory[(EffectManager.Effect)i];
         }

         for (var i = 0; i < EffectManager.instance.appliedEffects.Length; i++)
         {
            if (EffectManager.instance.appliedEffects[i] != EffectManager.Effect.None)
            {
               menu.transform.GetChild(i+7).GetChild(0).GetComponent<TextMeshProUGUI>().text = EffectManager.instance.appliedEffects[i] + "\n\n" + EffectManager.instance.effectInventory[EffectManager.instance.appliedEffects[i]];
            }
         }
      }
      
      public void BuyEffect(int buttonNumber)
      {
         if (effectsInTheShop[buttonNumber] != EffectManager.Effect.None && PlayerController.instance.currentEp >=
             effectManager.effectDictionary[(int)effectsInTheShop[buttonNumber]][
                EffectManager.instance.effectInventory[effectsInTheShop[buttonNumber]]].cost)
         {
            currentNumberOfTakenObjects++;
            EffectManager.instance.effectInventory[effectsInTheShop[buttonNumber]]++;
            PlayerController.instance.currentEp -=
               effectManager.effectDictionary[(int)effectsInTheShop[buttonNumber]][
                  EffectManager.instance.effectInventory[effectsInTheShop[buttonNumber]] - 1].cost;
            effectsInTheShop[buttonNumber] = EffectManager.Effect.None;
            for (var j = 0; j < 3; j++)
            {
               shopPanel.transform.GetChild(buttonNumber+1).GetChild(j).GetComponent<TextMeshProUGUI>().text = "Closed.";
            }

            if (currentNumberOfTakenObjects > maxNumberOfTakenObjects)
            {
               CloseShop();
               PlayerController.Die();
            }
         }
      }
      
      public void SelectEffectEmplacement(int buttonNumber)
      {
         selectedEffectEmplacement = buttonNumber;
         isEffectEmplacementSelected = true;
         canChooseEffect = true;
      }
      
      public void SelectEffect(int buttonNumber)
      {
         if (isEffectEmplacementSelected && EffectManager.instance.effectInventory[(EffectManager.Effect)buttonNumber] > 0 && canChooseEffect && !effectManager.appliedEffects.Contains((EffectManager.Effect)buttonNumber))
         {
            EffectManager.instance.appliedEffects[selectedEffectEmplacement] = (EffectManager.Effect)buttonNumber;
            whipModificationMenu.transform.GetChild(selectedEffectEmplacement+7).GetChild(0).GetComponent<TextMeshProUGUI>().text = EffectManager.instance.appliedEffects[selectedEffectEmplacement] + "\n\n" + EffectManager.instance.effectInventory[EffectManager.instance.appliedEffects[selectedEffectEmplacement]];
            canChooseEffect = false;
         }
      }
   }
}
