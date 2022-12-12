using UnityEngine;

namespace GenPro.Rooms
{
    public class DoorDetector : MonoBehaviour
    {
        [SerializeField] private GameObject DoorUp;
        [SerializeField] private GameObject DoorBottom;
        [SerializeField] private GameObject DoorLeft;
        [SerializeField] private GameObject DoorRight;
        
        [SerializeField] private GameObject Close_DoorUp;
        [SerializeField] private GameObject Close_DoorBottom;
        [SerializeField] private GameObject Close_DoorLeft;
        [SerializeField] private GameObject Close_DoorRight;

        public void ManageDoors(bool canUp, bool canBottom, bool canRight, bool canLeft)
        {
            DoorUp.SetActive(canUp);
            DoorBottom.SetActive(canBottom);
            DoorLeft.SetActive(canLeft);
            DoorRight.SetActive(canRight);
            
            Close_DoorUp.SetActive(!canUp);
            Close_DoorBottom.SetActive(!canBottom);
            Close_DoorLeft.SetActive(!canLeft);
            Close_DoorRight.SetActive(!canRight);
        }
    }
}