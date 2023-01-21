using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutoController : MonoBehaviour
{
      [Header("References")]
      [SerializeField] private GameObject tutoMenu;
      public GameObject tutoCanvas;
      public Image image;

      [SerializeField] private float timeToAccess;

      private float timerInputPressed;
      private bool isInTrigger;

      private UIManager uiManager;
      private bool isTutoOpen;
      public UiAnimTuto uiAnimTuto;
      [SerializeField] private AnimationClip tutoAnimClip;

      private void Start()
      {
         StartCoroutine(BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp());
         tutoCanvas.SetActive(false);

         timerInputPressed = 0f;
         uiManager = UIManager.instance;
         isTutoOpen = false;
      }

      private IEnumerator BecauseIAmReallyIrritatingSoINeedAFewTimeToWakeUp()
      {
         yield return new WaitForSeconds(0.3f);
         tutoMenu.SetActive(false);
      }

      private void OnTriggerStay2D(Collider2D col)
      {
         if (col.gameObject.CompareTag("Player"))
         {
            tutoCanvas.SetActive(true);

            isInTrigger = true;
         }
      }

      private void OnTriggerExit2D(Collider2D other)
      {
         if (other.gameObject.CompareTag("Player"))
         {
            timerInputPressed = 0f;
            tutoCanvas.SetActive(false);

            image.fillAmount = 0f;

            isInTrigger = false;
         }
      }

      private void Update()
      {
         if (tutoCanvas == true && isInTrigger)
         {
            OpenTutoMenu();
         }
         
         if (Input.GetKeyDown(InputManager.instance.quitKey) && isTutoOpen)
         {
            CloseTutoMenu();
         }
      }

      private void OpenTutoMenu()
      {
         if (Input.GetKey(InputManager.instance.interactKey) && !uiManager.IsAnyMenuOpened() && !PlayerController.instance.isRevealingDashOn && !PlayerController.instance.chainBlade.isWarningOn)
         {
            timerInputPressed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(0, 1, timerInputPressed / timeToAccess);

            if (timerInputPressed > timeToAccess - 0.05f && !isTutoOpen)
            {
               isTutoOpen = true;
               uiManager.isWhipMenuOpened = true;
               
               uiManager.DesactivateInGameUI();
               
               tutoMenu.SetActive(true); // si c'était un Canvas shopPanel.enabled = true;
               uiAnimTuto.OpenMenu();
               Time.timeScale = 0;
            }
         }

         if (Input.GetKeyUp(InputManager.instance.interactKey))
         {
            timerInputPressed = 0f;
            image.DOFillAmount(0,0.5f);
         }
      }
      
      private IEnumerator DisableIsTutoOpen()
      {
         yield return new WaitForSecondsRealtime(0.01f);
         uiManager.isWhipMenuOpened = false;
      }

      public void CloseTutoMenu()
      {
         isTutoOpen = false;
         StartCoroutine(DisableIsTutoOpen());
         uiAnimTuto.CloseMenu();
         StartCoroutine(DisableTutoOpenCoroutine());
         
         uiManager.ActivateInGameUI();

         image.fillAmount = 0f;

         Time.timeScale = 1;
      }

      private IEnumerator DisableTutoOpenCoroutine()
      {
         yield return new WaitForSecondsRealtime(tutoAnimClip.length);
         uiManager.isWhipMenuOpened = false;
      }
}
