using System;
using System.Collections;
using Controller;
using Core.Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

namespace Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {
        internal new static UIManager instance;
    
        [SerializeField] private TMP_Text epCount;
        private InputManager inputManager;
        private ScoreManager scoreManager;
        private PlayerController playerController;

        [Header("Map & MiniMap")] 
        //public Image mapPanel;
        public GameObject inGameUI;
        //public GameObject bigMapRender;
        //public GameObject bigMapCanvas;
        //public Camera camBigMap;
    
        [Header("Menus")]
        public static GameObject pauseMenu;
        public static GameObject settingsMenu;
        public GameObject gameOverMenu;
        //public GameObject winMenu;

        private static bool _isGamePausedStatic;
        internal bool isGamePaused;
        internal bool isShopOpened;
        internal bool isWhipMenuOpened;

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
            _isGamePausedStatic = false;
            isShopOpened = false;
            isWhipMenuOpened = false;
            //Reference
        
            //BIG MAP
            inGameUI = GameObject.Find("InGameUI");
            gameOverMenu = GameObject.Find("GameOverScreen");
            //camBigMap = GameObject.Find("BigMapCamera").GetComponent<Camera>();
            //bigMapRender = GameObject.Find("BigMapRender");
            scoreManager = ScoreManager.instance;
        
            inputManager = InputManager.instance;
            epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TMP_Text>();

            //bigMapCanvas = GameObject.Find("BigMapCanvas");
            //mapPanel = bigMapCanvas.GetComponent<Image>();
            pauseMenu = GameObject.Find("PauseMenu");
            settingsMenu = GameObject.Find("OptionsMenu");
        
            epCount.text = "EP COUNT : " + 0;
        
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            settingsMenu.SetActive(false);
            //bigMapRender.SetActive(false);
            //scoreManager.scoreText.enabled = false;

            StartCoroutine(WaitForPlayer());
        }

        private void Update()
        {
            /*if (Input.GetKey(inputManager.mapKey))
            {
                bigMapCanvas.SetActive(true);
                //scoreManager.scoreText.enabled = true;
                camBigMap.enabled = true;
                bigMapRender.SetActive(true);
                camBigMap.orthographicSize = 300;
            
                inGameUI.SetActive(false);
                mapPanel.DOFade(0.7f, 0.3f);
            
            }
            else
            {
                //scoreManager.scoreText.enabled = false;
                camBigMap.enabled = false;
                bigMapRender.SetActive(false);
                camBigMap.orthographicSize = 0;
                
                mapPanel.DOFade(0f, 0.3f);
                bigMapCanvas.SetActive(false);
            }
            if (Input.GetKeyUp(inputManager.mapKey))
            {
                inGameUI.SetActive(true);
            }*/
        
            PauseMenuInput();
            isGamePaused = _isGamePausedStatic;
            
            if (playerController is null)
                return;
            
            epCount.text = playerController.currentEp.ToString();
        }

        #region PauseMenu

        private void PauseMenuInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isShopOpened && !isWhipMenuOpened)
            {
                _isGamePausedStatic = !_isGamePausedStatic;
                PauseMenu(_isGamePausedStatic);
            }
        }

        private static void PauseMenu(bool isGamePaused)
        {
            pauseMenu.SetActive(isGamePaused);
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
            _isGamePausedStatic = false;
            PauseMenu(_isGamePausedStatic);
        }
        
        #endregion

        public void GameOver()
        {
            DesactivateInGameUI();
            gameOverMenu.SetActive(true);
        }

        public void ReloadLevel()
        {
            LoadingScreen.instance.ShowLoadingScreen();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        public void BackToMenu()
        {
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
            settingsMenu.SetActive(true);
        }
    }
}
