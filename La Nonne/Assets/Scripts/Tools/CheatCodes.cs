using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using GenPro.Rooms.Generator;
using Unity.VisualScripting;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SO_Controller controller;
    [SerializeField] private RoomContentGenerator roomContentGenerator;
    
    private void Start()
    {
        StartCoroutine(GetPlayer());
    }

    public void GodMode()
    {
        playerController.isGodModeOn = !playerController.isGodModeOn;
    }

    public void TpToBoss()
    {
        var boss = roomContentGenerator.mapBoss;
        playerController.transform.position = new Vector2(boss.x, boss.y);
    }

    public void TpToShop()
    {
        var shop = roomContentGenerator.lastShopPosition;
        playerController.transform.position = new Vector2(shop.x, shop.y);
    }

    public void MoneyGlitch()
    {
        playerController.currentEp += 100;
    }

    private IEnumerator GetPlayer()
    {
        bool Condition() => PlayerController.instance != null;
        yield return new WaitUntil(Condition);
        playerController = PlayerController.instance;
        controller = playerController.soController;
    }
}
