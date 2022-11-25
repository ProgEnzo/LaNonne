using Controller;
using TMPro;
using UnityEngine;

namespace Shop
{
    public class UpgradeButton : MonoBehaviour
    {
        public PlayerController playerController;
        public int upgradeNumber;

        public TextMeshProUGUI effectName;
        public TextMeshProUGUI cost;
        public TextMeshProUGUI description;

        public void Start()
        {
        
        }

        void SetButton()
        {
            //mettre les array ici
        }

        public void OnClick()
        {
            //verifier si le joueur a assez de tune pour acheter
        }
    }
}
