using Core.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : MonoSingleton<ScoreManager>
    {
        internal new static ScoreManager instance;
        private int score;
    
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
        }

        private void Update()
        {
            scoreText.text = "Score : " + score;
        }

        public void AddScore(int addedScore)
        {
            score += addedScore;

            if (score < 0)
            {
                score = 0;
            }
        }
    }
}
