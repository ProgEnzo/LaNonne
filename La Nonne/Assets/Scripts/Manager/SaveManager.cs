using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using File = System.IO.File;

namespace Manager
{
    public static class SaveManager
    {
        internal static void SaveData(List<int> data, int newData)
        {
            if (data != null)
            {
                var count = data.Count;
                
                for (var i = 0; i < count; i++)
                {
                    if (newData > data[i])
                    {
                        data.Insert(i, newData);
                        break;
                    }

                    if (i == count - 1)
                    {
                        data.Add(newData);
                    }
                }

                if (count > 5)
                {
                    data.RemoveAt(5);
                }
            }
            else
            {
                data = new List<int> {newData};
                for (var i = 0; i < 4; i++)
                {
                    data.Add(0);
                }
            }

            File.WriteAllLines(Application.dataPath + "/data.txt", data.ConvertAll(x => x.ToString()));
        }
        
        internal static List<int> LoadData()
        {
            return File.Exists(Application.dataPath + "/data.txt") ? 
                File.ReadAllLines(Application.dataPath + "/data.txt").ToList().Select(int.Parse).ToList() : 
                null;
        }
    }
}
