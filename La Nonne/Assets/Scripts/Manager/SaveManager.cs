using System;
using System.IO;
using Core.Scripts.Utils;
using UnityEngine;
using File = System.IO.File;

namespace Manager
{
    public class SaveManager : MonoSingleton<SaveManager>
    {
        internal new static SaveManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                instance = this;
            }

            LoadData();
            SaveData();
        }
        
        private void SaveData()
        {
            File.WriteAllText(Application.dataPath + "/data.txt", "Hello World");
        }
        
        private void LoadData()
        {
            if (File.Exists(Application.dataPath + "/data.txt"))
            {
                var data = File.ReadAllText(Application.dataPath + "/data.txt");
                Debug.Log(data);
            }
        }
    }
}
