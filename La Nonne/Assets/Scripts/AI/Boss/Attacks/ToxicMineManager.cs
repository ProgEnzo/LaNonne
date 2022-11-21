using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicMineManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DealsDamage());
    }

    private IEnumerator DealsDamage()
    {
        
        yield return new WaitForSeconds(1f);
    }
}
