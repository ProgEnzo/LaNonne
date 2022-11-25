using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
   [Header("References")] 
   private GameObject shopPanel;
   private GameObject whipModifMenu;
   public GameObject shopCanvas;
   public Image image;
   
   private float timeToAccess = 4f;

   private float timerInputPressed;
   private bool isInTrigger;

   private void Start()
   {
      StartCoroutine(JeTeBaise());
      shopCanvas.SetActive(false);

      //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!

      timerInputPressed = 0f;
   }

   private IEnumerator JeTeBaise()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      whipModifMenu = GameObject.FindGameObjectWithTag("WhipModifMenu");
      yield return new WaitForSeconds(0.3f);
      shopPanel.SetActive(false);
      whipModifMenu.SetActive(false);
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

   public void OpenShop()
   {
      if (Input.GetKey(KeyCode.E))
      {
         timerInputPressed += Time.deltaTime;
         image.fillAmount = Mathf.Lerp(0, 1, timerInputPressed / timeToAccess);

         if (timerInputPressed > timeToAccess - 0.05f)
         {
            shopPanel.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
            Time.timeScale = 0;
         }
      }

      if (Input.GetKeyUp(KeyCode.E))
      {
         timerInputPressed = 0f;
         image.DOFillAmount(0,0.5f);
      }
      
   }

   public void CloseShop()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      shopPanel.SetActive(false);

      image.fillAmount = 0f;

      Time.timeScale = 1;  
   }
   
   public void CloseWhipModifMenu()
   {
      whipModifMenu = GameObject.FindGameObjectWithTag("WhipModifMenu");
      whipModifMenu.SetActive(false);
      
      //shopPanel.SetActive(true);
   }

   public void OpenWhipModifMenu(GameObject x)
   {
      x.SetActive(true);
   }
   
}
