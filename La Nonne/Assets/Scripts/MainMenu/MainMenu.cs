using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void LoadGame(string level)
   {
      SceneManager.LoadScene(level);
   }

   public void Quit()
   {
      Application.Quit();
   }
}
