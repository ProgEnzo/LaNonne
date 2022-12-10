using System;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI epCount;
    private InputManager inputManager;

    [Header("Map & MiniMap")] 
    public Image mapPanel;
    private void Start()
    {
        //Reference
        inputManager = InputManager.instance;
        epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        // mapPanel = GameObject.Find("ScorePanel").GetComponent<Image>();
        
        epCount.text = "EP COUNT : " + 0;
    }

    private void Update()
    {
        if (Input.GetKey(inputManager.mapKey))
        {
            mapPanel.DOFade(0.7f, 0.3f);
            
        }
        else
        {
            mapPanel.DOFade(0f, 0.3f);
        }
    }
}
