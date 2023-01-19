using System.Collections;
using Controller;
using Core.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {
        internal new static UIManager instance;
    
        [SerializeField] private TMP_Text epCount;
        private InputManager inputManager;
        private PlayerController playerController;
        private ChainBlade chainBlade;
        
        [Header("Map & MiniMap")] 
        public GameObject inGameUI;

        public static GameObject settingsMenu;
        public GameObject gameOverMenu;

        internal static bool _isGamePausedStatic;
        internal bool isGamePaused;
        internal bool isShopOpened;
        internal bool isWhipMenuOpened;
        internal bool isGameOver;

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
            
            inGameUI = GameObject.Find("InGameUI");
            gameOverMenu = GameObject.Find("GameOverScreen");
        
            inputManager = InputManager.instance;
            epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TMP_Text>();

            GameObject.Find("PauseMenu");
            settingsMenu = GameObject.Find("OptionsMenu");
        
            epCount.text = "EP COUNT : " + 0;
        
            gameOverMenu.SetActive(false);
            settingsMenu.SetActive(false);

            StartCoroutine(WaitForPlayer());
        }

        private void Update()
        {
            PauseMenuInput();
            isGamePaused = _isGamePausedStatic;
            
            if (playerController is null)
                return;
            
            epCount.text = playerController.currentEp.ToString();
        }

        #region PauseMenu

        private void PauseMenuInput()
        {
            if (Input.GetKeyDown(inputManager.pauseKey) && !isShopOpened && !isWhipMenuOpened && !isGameOver)
            {
                _isGamePausedStatic = !_isGamePausedStatic;
                PauseMenu(_isGamePausedStatic);
            }
        }

        public static void PauseMenu(bool isGamePaused)
        {
            //pauseMenu.SetActive(isGamePaused);
            _isGamePausedStatic = isGamePaused;
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
            _isGamePausedStatic = false;
            PauseMenu(_isGamePausedStatic);
        }
        
        #endregion

        public void GameOver()
        {
            DesactivateInGameUI();
            gameOverMenu.SetActive(true);
            isGameOver = true;
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
            settingsMenu.SetActive(true);
        }
        
        internal bool IsAnyMenuOpened()
        {
            return isGamePaused || isGameOver || isShopOpened || isWhipMenuOpened;
        }
    }
}
