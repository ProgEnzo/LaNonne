using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ShopController : MonoBehaviour
{
   private GameObject shopPanel;

   public RectTransform image;

   private void Start()
   {
      StartCoroutine(JeTeBaise());

      //image.DORectTransformMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutFlash); Merci Mathieu je vais voir ca ce soir !!
   }

   private IEnumerator JeTeBaise()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      yield return new WaitForSeconds(0.5f);
      shopPanel.SetActive(false);
   }

   private void OnTriggerEnter2D(Collider2D col)
   {
      if (col.gameObject.CompareTag("Player"))
         OpenShop();
   }

   void OpenShop()
   {
      shopPanel.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
      Time.timeScale = 0;
   }

   public void CloseShop()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
      shopPanel.SetActive(false);
      Time.timeScale = 1;  
   }
   
}
