using Core.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : MonoSingleton<ScoreManager>
    {
        internal new static ScoreManager instance;
        
        private int score;
        private int killedEnemyScore;
        private int bladeHitScore;
        private int chainBladeHitScore;
        private int revealingDashBladeHitScore;
        private int takenHitScore;
        private int boughtItemScore;
        private int epScore;
        private int timeScore;

        private TextMeshProUGUI scoreText;
        
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
        
        private void Start()
        {
            scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
            
            score = 0;
            killedEnemyScore = 0;
            bladeHitScore = 0;
            chainBladeHitScore = 0;
            revealingDashBladeHitScore = 0;
            takenHitScore = 0;
            boughtItemScore = 0;
            epScore = 0;
            timeScore = 0;
        }

        private void Update()
        {
            scoreText.text = "Score : " + score;
        }
        
        internal void AddKilledEnemyScore(int scoreToAdd)
        {
            killedEnemyScore += scoreToAdd;
            
            UpdateScore();
        }
        
        internal void AddBladeHitScore(int scoreToAdd)
        {
            bladeHitScore += scoreToAdd;
            
            UpdateScore();
        }
        
        internal void AddChainBladeHitScore(int scoreToAdd)
        {
            chainBladeHitScore += scoreToAdd;
            
            UpdateScore();
        }
        
        internal void AddRevealingDashBladeHitScore(int scoreToAdd)
        {
            revealingDashBladeHitScore += scoreToAdd;
            
            UpdateScore();
        }
        
        internal void AddTakenHitScore(int scoreToAdd)
        {
            if (score > 0)
            {
                takenHitScore += Mathf.Max(scoreToAdd, -score);
            }
            
            UpdateScore();
        }
        
        internal void AddBoughtItemScore(int scoreToAdd)
        {
            boughtItemScore += scoreToAdd;
            
            UpdateScore();
        }
        
        internal void AddEpScore(int scoreToAdd)
        {
            epScore += scoreToAdd;
            
            UpdateScore();
        }
        
        internal void SetTimeScore()
        {
            timeScore = (int) -Time.time;
            
            UpdateScore();
        }

        private void UpdateScore()
        {
            score = killedEnemyScore + bladeHitScore + chainBladeHitScore + revealingDashBladeHitScore + takenHitScore + boughtItemScore + epScore + timeScore;

            if (score < 0)
            {
                score = 0;
            }
        }
    }
}
