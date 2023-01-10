using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealingJar : MonoBehaviour
{
    public GameObject healDrop;
    private GameObject healDropObject;
    public int numberOfHealDrops;
    private CamManager _camManager;

    public List<GameObject> healDropList = new();
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Blade"))
        {
            CamManager.instance.DestroyingHealthJarState(1);
            
            for (int i = 0; i < numberOfHealDrops; i++)
            {
                healDropObject = Instantiate(healDrop, transform.position, Quaternion.identity);
                
                healDropList.Add(healDropObject);
                
                Destroy(gameObject);
            }                

        }
    }
}
