using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
   public class MainMenu : MonoBehaviour
   {
      [SerializeField] private GameObject settingsMenu;
      [SerializeField] private Texture2D mainCursorTexture;

      private void Awake()
      {
         //Cursor
         Cursor.SetCursor(mainCursorTexture, Vector2.zero, CursorMode.Auto);
         Cursor.lockState = CursorLockMode.Confined;
      }

      private void Start()
      {
         settingsMenu.SetActive(false);
      }

      public void LoadGame(string level)
      {
         LoadingScreen.instance.ShowLoadingScreen();
         SceneManager.LoadScene(level);
      }

      public void OptionsMenu()
      {
         settingsMenu.SetActive(true);
      }

      public void BackFromSettings()
      {
         settingsMenu.SetActive(false);
      }

      public void Quit()
      {
         Application.Quit();
      }
   }
}
