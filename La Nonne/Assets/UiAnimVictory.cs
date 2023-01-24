using Core.Scripts.Utils;
using DG.Tweening;
using Manager;
using UnityEngine;

public class UiAnimVictory : MonoSingleton<UiAnimVictory>
{
    internal new static UiAnimVictory instance;
    
    public Animator anim;
    private bool menuOpen = false;
    public GameObject isWin;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Jiggle()
    {
        isWin.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }
  
    public void OpenMenu()
    {
        if (!menuOpen)
        {
            UIManager.instance.isVictory = true;
            anim.SetBool("isWin", true);
            menuOpen = true;
            
            UIManager.instance.PrintDetailedScore();
            SaveManager.SaveData(SaveManager.LoadData(), ScoreManager.instance.ScoreSwitch(-1));
        }
    }
}
