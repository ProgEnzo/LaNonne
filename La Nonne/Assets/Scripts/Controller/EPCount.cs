using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using TMPro;
using UnityEngine;

public class EPCount : MonoBehaviour
{
    [SerializeField] private int epValue;
    [SerializeField] private TextMeshProUGUI epCount;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        playerController = PlayerController.instance;
        epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }        


    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerController.AddEP(epValue);
            epCount.text = "EP COUNT : " + playerController.currentEp;

            Destroy(gameObject);
        }
    }
    
    
}
