using System;
using Cinemachine;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI epCount;
    private InputManager inputManager;
    private ScoreManager scoreManager;

    [Header("Map & MiniMap")] 
    public Image mapPanel;
    public GameObject inGameUI;
    public GameObject bigMapRender;
    public Camera camBigMap;
    
    [Header("Menus")]
    public GameObject pauseMenu;
    //public GameObject settingsMenu;
    public GameObject gameOverMenu;
    //public GameObject winMenu;
    private void Start()
    {
        //Reference
        
        //BIG MAP
        inGameUI = GameObject.Find("InGameUI");
        camBigMap = GameObject.Find("BigMapCamera").GetComponent<Camera>();
        bigMapRender = GameObject.Find("BigMapRender");
        scoreManager = ScoreManager.instance;
        
        inputManager = InputManager.instance;
        epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        mapPanel = GameObject.Find("BigMapCanvas").GetComponent<Image>();
        
        epCount.text = "EP COUNT : " + 0;
        
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        bigMapRender.SetActive(false);
        //scoreManager.scoreText.enabled = false;

    }

    private void Update()
    {
        if (Input.GetKey(inputManager.mapKey))
        {
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
            
            inGameUI.SetActive(true);
            mapPanel.DOFade(0f, 0.3f);
        }
        
        PauseMenuOn();
    }

    public void PauseMenuOn()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
