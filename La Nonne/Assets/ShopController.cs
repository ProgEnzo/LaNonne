using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShopController : MonoBehaviour
{
   private GameObject shopPanel;

   private void Start()
   {
      StartCoroutine(JeTeBaise());
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
