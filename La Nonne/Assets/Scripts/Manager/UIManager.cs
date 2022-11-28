using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI epCount;
    
    private void Start()
    {
        epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        epCount.text = "EP COUNT : " + 0;
    }
}
