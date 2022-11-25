using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
   public void LoadGame(string level)
   {
      Application.LoadLevel(level);
   }

   public void Quit()
   {
      Application.Quit();
   }
}
