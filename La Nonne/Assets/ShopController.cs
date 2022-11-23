using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShopController : MonoBehaviour
{
   public GameObject shopPanel;

   private void Start()
   {
      shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
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

   void CloseShop()
   {
      shopPanel.SetActive(false);
      Time.timeScale = 1;  
   }
   
}
