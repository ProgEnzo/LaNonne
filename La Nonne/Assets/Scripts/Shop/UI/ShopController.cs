using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Manager;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Shop.UI
{
   public class ShopController : MonoBehaviour
   {
      [Header("References")]
      [SerializeField] private GameObject shopPanel;
      public GameObject shopCanvas;
      public Image image;
      [SerializeField] private TextMeshProUGUI epCountText;
   
      [SerializeField] private float timeToAccess;

      private float timerInputPressed;
      private bool isInTrigger;

      [SerializeField] private int numberOfOfferedObjects;
      private EffectManager.Effect[] effectsInTheShop;
      private UIManager uiManager;
      private EffectManager effectManager;
      private bool isShopOpened;
      private bool hasShopBeenOpened;
      [SerializeField] private int maxNumberOfTakenObjects;
      private int currentNumberOfTakenObjects;

      private ScoreManager scoreManager;
      
      private DialoguesManager dialoguesManager;
      [SerializeField] private TextMeshProUGUI dialogueText;
      [SerializeField] private float timeByCharacter;

      private void Start()
      {
         StartCoroutine(BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp());
         shopCanvas.SetActive(false);

         //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!

         timerInputPressed = 0f;
         effectManager = EffectManager.instance;
         uiManager = UIManager.instance;
         scoreManager = ScoreManager.instance;
         dialoguesManager = DialoguesManager.instance;
         hasShopBeenOpened = false;
         isShopOpened = false;
      }

      private IEnumerator BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp()
      {
         yield return new WaitForSeconds(0.3f);
         shopPanel.SetActive(false);
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
            OpenShop();
         }
         
         epCountText.text = "EP : " + PlayerController.instance.currentEp;
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
                  PlayerController.instance.visitedShopCount += 1;
                  currentNumberOfTakenObjects = 0;
                  ShopObjectsSelector();
               
                  //Mise en place du dialogue
                  ChooseDialogue();
               }
               
               //Mise en place des objets dans le shop
               for (var i = 0; i < effectsInTheShop.Length; i++)
               {
                  if (effectsInTheShop[i] != EffectManager.Effect.None)
                  {
                     shopPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite =
                        effectManager.effectDictionary[(int)effectsInTheShop[i]][
                           EffectManager.instance.effectInventory[effectsInTheShop[i]]].image;
                     shopPanel.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                        effectsInTheShop[i] + " - T" +
                        (EffectManager.instance.effectInventory[effectsInTheShop[i]] + 1);
                     shopPanel.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                        "Cost : " + effectManager.effectDictionary[(int)effectsInTheShop[i]][
                           EffectManager.instance.effectInventory[effectsInTheShop[i]]].cost;
                     shopPanel.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text =
                        effectManager.effectDictionary[(int)effectsInTheShop[i]][
                           EffectManager.instance.effectInventory[effectsInTheShop[i]]].description;
                  }
                  else
                  {
                     for (var j = 0; j < 3; j++)
                     {
                        shopPanel.transform.GetChild(i).GetChild(j).GetComponent<TextMeshProUGUI>().text = "Closed.";
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

      private void ChooseDialogue()
      {
         var chosenList = PlayerController.instance.visitedShopCount switch
         {
            <= 2 => dialoguesManager.currentTexts1,
            <= 4 => dialoguesManager.currentTexts2,
            _ => dialoguesManager.currentTexts3
         };
         
         var chosenDialogue = chosenList[Random.Range(0, chosenList.Count)];

         StartCoroutine(WriteDialogue(chosenDialogue));
         
         chosenList.Remove(chosenDialogue);
      }
      
      private IEnumerator WriteDialogue(string text)
      {
         dialogueText.text = "";
         
         foreach (var character in text)
         {
            dialogueText.text += character;
            yield return new WaitForSecondsRealtime(timeByCharacter);
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
            
            //ADD SCORE FOR BUYING ITEMS
            scoreManager.AddBoughtItemScore(100);
            
            effectsInTheShop[buttonNumber] = EffectManager.Effect.None;
            for (var j = 0; j < 3; j++)
            {
               shopPanel.transform.GetChild(buttonNumber).GetChild(j+1).GetComponent<TextMeshProUGUI>().text = "Closed.";
            }

            if (currentNumberOfTakenObjects > maxNumberOfTakenObjects)
            {
               CloseShop();
               PlayerController.Die();
            }
         }
      }
   }
}
