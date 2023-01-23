using System.Collections.Generic;
using Manager;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    [SerializeField] private List<string> recordNames;

    private void Start()
    {
        var loadedData = SaveManager.LoadData();
        if (loadedData != null)
        {
            for (var i = loadedData.Count; i < recordNames.Count; i++)
            {
                loadedData.Add(0);
            }

            for (var i = 0; i < recordNames.Count; i++)
            {
                transform.GetChild(i+2).GetComponent<TMPro.TextMeshProUGUI>().text = recordNames[i] + loadedData[i];
            }
        }
        else
        {
            for (var i = 0; i < recordNames.Count; i++)
            {
                transform.GetChild(i+2).GetComponent<TMPro.TextMeshProUGUI>().text = recordNames[i] + "0";
            }
        }
    }
}
