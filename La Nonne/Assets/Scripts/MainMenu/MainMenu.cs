using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public GameObject SettingsMenu;

   private void Start()
   {
      SettingsMenu.SetActive(false);
   }

   public void LoadGame(string level)
   {
      LoadingScreen.instance.ShowLoadingScreen();
      SceneManager.LoadScene(level);
   }

   public void OptionsMenu()
   {
      SettingsMenu.SetActive(true);
   }

   public void BackFromSettings()
   {
      SettingsMenu.SetActive(false);
   }

   public void Quit()
   {
      Application.Quit();
   }
}
