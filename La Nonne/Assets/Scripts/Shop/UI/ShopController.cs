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
   public GameObject shopCanvas;
   public Image image;
   
   private float timeToAccess = 0f;

   

   private void Start()
   {
      StartCoroutine(JeTeBaise());
      shopCanvas.SetActive(false);

      //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!
   }

   private IEnumerator JeTeBaise()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      yield return new WaitForSeconds(0.3f);
      shopPanel.SetActive(false);
   }

   private void OnTriggerStay2D(Collider2D col)
   {
      if (col.gameObject.CompareTag("Player"))
      {
         shopCanvas.SetActive(true);
      }

      if (shopCanvas == true)
      {
         OpenShop();
      }
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         image.DOFillAmount(0, 0f);
         shopCanvas.SetActive(false);
      }
   }

   /*private void Update()
   {
      OpenShop();
   }*/

   public void OpenShop()
   {
      if (Input.GetKey(KeyCode.E))
      {
         timeToAccess += 0.1f;
         image.DOFillAmount(timeToAccess, 0f);

         if (timeToAccess == 0.5f)
         {
            shopPanel.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
            Time.timeScale = 0;
         }
      }

      if (Input.GetKeyUp(KeyCode.E))
      {
         timeToAccess = 0f;
         image.DOFillAmount(0, 0f);
      }
      
   }

   public void CloseShop()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      shopPanel.SetActive(false);
      image.DOFillAmount(0, 0f);

      Time.timeScale = 1;  
   }
   
}
