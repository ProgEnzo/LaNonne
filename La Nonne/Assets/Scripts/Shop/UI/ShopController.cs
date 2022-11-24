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
   //public Canvas shopCanvas;
   public Image image;
   
   private float timeToAccess;

   

   private void Start()
   {
      StartCoroutine(JeTeBaise());

      //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!
   }

   private IEnumerator JeTeBaise()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      //shopCanvas = Canvas.FindObjectOfType<Canvas>(false);
      yield return new WaitForSeconds(0.3f);
      shopPanel.SetActive(false);
   }

   /*private void OnTriggerEnter2D(Collider2D col)
   {
      if (col.gameObject.CompareTag("Player"))
      {
         shopCanvas.enabled = true;
         OpenShop();
      }
   }*/

   private void Update()
   {
      OpenShop();
   }

   public void OpenShop()
   {
      if (Input.GetKey(KeyCode.E))
      {
         timeToAccess += 0.1f;
         image.DOFillAmount(timeToAccess, 1f);

         if (timeToAccess == 0.5f)
         {
            shopPanel.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
            Time.timeScale = 0;
         }
      }

      if (Input.GetKeyUp(KeyCode.E))
      {
         timeToAccess = 0f;
      }
      
   }

   public void CloseShop()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      shopPanel.SetActive(false);
      Time.timeScale = 1;  
   }
   
}
