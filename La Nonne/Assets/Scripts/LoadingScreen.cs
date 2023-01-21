using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]

public class LoadingScreen : MonoSingleton<LoadingScreen>
{
    private CanvasGroup _canvasGroup;
    public AnimLoadingScript animLoadingScript;
    private int x;
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }
    
    public void ShowLoadingScreen()
    {
        _canvasGroup.alpha = 1;
        animLoadingScript.OpenMenu();
        
    }
    
    public void HideLoadingScreen()
    {
        _canvasGroup.alpha = 0;
    }
}
