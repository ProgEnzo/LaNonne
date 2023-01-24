using System.Collections;
using System.Collections.Generic;
using Controller;
using Core.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("Instance")]
        internal new static UIManager instance;
    
        [Header("Cursor textures")]
        [SerializeField] private Texture2D mainCursorTexture;
        [SerializeField] private Texture2D gameOverCursorTexture;
        
        [Header("Script references")]
        private InputManager inputManager;
        private ScoreManager scoreManager;
        private PlayerController playerController;
        
        [Header("Menus")]
        private GameObject inGameUI;
        private static GameObject _settingsMenu;
        private GameObject gameOverMenu;
        [SerializeField] private GameObject detailedScoreMenuTextsGameObject;

        [Header("Texts")]
        [SerializeField] private TMP_Text epCount;
        [SerializeField] private TMP_Text globalScoreText;
        private readonly List<TMP_Text> detailedScoreMenuTexts = new();
        [SerializeField] private List<string> detailedScoreMenuMainTexts = new();
        
        [Header("Booleans")]
        internal static bool isGamePausedStatic;
        internal bool isGamePaused;
        internal static bool isSettingsOn;
        internal bool isShopOpened;
        internal bool isWhipMenuOpened;
        internal bool isTutoOpened;
        private bool isGameOver;
        internal bool isVictory;
        internal bool isCinematicOn;

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
            
            //Cursor
            Cursor.SetCursor(mainCursorTexture, Vector2.zero, CursorMode.Auto);
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Start()
        {
            isGamePausedStatic = false;
            isShopOpened = false;
            isWhipMenuOpened = false;
            
            inGameUI = GameObject.Find("InGameUI");
            gameOverMenu = GameObject.Find("GameOverScreen");
        
            inputManager = InputManager.instance;
            scoreManager = ScoreManager.instance;
            epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TMP_Text>();

            GameObject.Find("PauseMenu");
            _settingsMenu = GameObject.Find("OptionsMenu");

            for (var i = 1; i < detailedScoreMenuTextsGameObject.transform.childCount; i++)
            { 
                detailedScoreMenuTexts.Add(detailedScoreMenuTextsGameObject.transform.GetChild(i).GetComponent<TMP_Text>());
            }
        
            gameOverMenu.SetActive(false);
            _settingsMenu.SetActive(false);

            StartCoroutine(WaitForPlayer());
        }

        private void Update()
        {
            PauseMenuInput();
            isGamePaused = isGamePausedStatic;
            
            if (playerController is null)
                return;
            
            epCount.text = playerController.currentEp.ToString();
        }

        #region PauseMenu

        private void PauseMenuInput()
        {
            if (Input.GetKeyDown(inputManager.pauseKey) && !isShopOpened && !isWhipMenuOpened && !isTutoOpened && !isGameOver && !isSettingsOn && !isVictory && !isCinematicOn)
            {
                isGamePausedStatic = !isGamePausedStatic;
                PauseMenu(isGamePausedStatic);
            }
        }

        public static void PauseMenu(bool isGamePaused)
        {
            //pauseMenu.SetActive(isGamePaused);
            isGamePausedStatic = isGamePaused;
            if (!isGamePaused)
            {
                UIAnimPause.instance.CloseMenu();
            }
            else
            {
                UIAnimPause.instance.OpenMenu();
            }
            
            Time.timeScale = isGamePaused ? 0 : 1;
            Time.fixedDeltaTime = 0.02f;
            if (PlayerController.instance.isRevealingDashOn)
            {
                PlayerController.instance.RevealingDashTimeSequenceTo1();
                PlayerController.instance.RevealingDashSpeedSequenceTo1();
                
                PlayerController.instance.isRevealingDashHitting = false;
                PlayerController.instance.isRevealingDashFocusOn = false;
                PlayerController.instance.isRevealingDashOn = false;
                CamManager.instance.ZoomDuringRevealingDash(0);
                PlayerController.instance.currentRevealingDashCooldown = PlayerController.instance.soController.revealingDashCooldown;
            }
        }
    
        public static void Resume()
        {
            isGamePausedStatic = false;
            PauseMenu(isGamePausedStatic);
        }
        
        #endregion

        public void GameOver()
        {
            DesactivateInGameUI();
            gameOverMenu.SetActive(true);
            isGameOver = true;
            
            //Detailed score
            PrintDetailedScore();
            SaveManager.SaveData(SaveManager.LoadData(), scoreManager.ScoreSwitch(-1));

            //Cursor
            Cursor.SetCursor(gameOverCursorTexture, Vector2.zero, CursorMode.Auto);
            
            Time.timeScale = 0;
        }

        public void ReloadLevel()
        {
            Time.timeScale = 1;
            LoadingScreen.instance.ShowLoadingScreen();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        public void BackToMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        internal void DesactivateInGameUI()
        {
            inGameUI.SetActive(false);
        }
    
        internal void ActivateInGameUI()
        {
            inGameUI.SetActive(true);
        }

        private IEnumerator WaitForPlayer()
        {
            bool Test() => PlayerController.instance == null;
            yield return new WaitWhile(Test);
            playerController = PlayerController.instance;
        }
        
        public static void OptionsMenu()
        {
            isSettingsOn = true;
            _settingsMenu.SetActive(true);
        }
        
        internal bool IsAnyMenuOpened()
        {
            return isGamePaused || isGameOver || isShopOpened || isWhipMenuOpened || isVictory || isCinematicOn;
        }

        internal void PrintDetailedScore()
        {
            globalScoreText.text = scoreManager.ScoreSwitch(-1).ToString();
            
            for (var i = 0; i < detailedScoreMenuTexts.Count; i++)
            {
                detailedScoreMenuTexts[i].text = detailedScoreMenuMainTexts[i] + scoreManager.ScoreSwitch(i);
            }
        }
    }
}
