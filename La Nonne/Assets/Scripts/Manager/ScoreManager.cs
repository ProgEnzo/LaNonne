using System;
using System.Collections;
using System.Collections.Generic;
using AI.So;
using Core.Scripts.Utils;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    internal new static ScoreManager instance;
    public int score;
    
    public TextMeshProUGUI scoreText;
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

    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = score.ToString();

    }
    
}
