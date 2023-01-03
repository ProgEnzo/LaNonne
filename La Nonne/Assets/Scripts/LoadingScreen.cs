using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]

public class LoadingScreen : MonoSingleton<LoadingScreen>
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject chargement1;
    [SerializeField] private GameObject chargement2;
    [SerializeField] private GameObject chargement3;
    private int x;
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        chargement1.SetActive(false);
        chargement2.SetActive(false);
        chargement3.SetActive(false);
        
    }
    
    public void ShowLoadingScreen()
    {
        StartCoroutine(Chargement());
        _canvasGroup.alpha = 1;
    }
    
    public void HideLoadingScreen()
    {
        _canvasGroup.alpha = 0;
    }

    private IEnumerator Chargement()
    {
        while (x < 100)
        {
            chargement1.SetActive(true);
            yield return new WaitForSeconds(1f);
            chargement1.SetActive(false);
            chargement2.SetActive(true);
            yield return new WaitForSeconds(1f);
            chargement2.SetActive(false);
            chargement3.SetActive(true);
            yield return new WaitForSeconds(1f);
            chargement3.SetActive(false);
            x += 1;
        }
    }
}
