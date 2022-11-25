using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    public PlayerController playerController;
    public int upgradeNumber;

    public TextMeshProUGUI name;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI description;

    public void Start()
    {
        
    }

    void SetButton()
    {
        //mettre les array ici
    }

    public void OnClick()
    {
        //verifier si le joueur a assez de tune pour acheter
    }
}
