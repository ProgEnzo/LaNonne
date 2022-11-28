using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI epCount;
    public Slider hpBossSlider;

    
    
    private void Start()
    {
        epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        epCount.text = "EP COUNT : " + 0;
        
        hpBossSlider = GameObject.FindGameObjectWithTag("Boss HealthBar").GetComponent<Slider>();
        hpBossSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
