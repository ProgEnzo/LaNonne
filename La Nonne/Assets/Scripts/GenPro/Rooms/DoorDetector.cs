using UnityEngine;

namespace GenPro.Rooms
{
    public class DoorDetector : MonoBehaviour
    {
        [SerializeField] private GameObject DoorUp;
        [SerializeField] private GameObject DoorBottom;
        [SerializeField] private GameObject DoorLeft;
        [SerializeField] private GameObject DoorRight;

        public void ManageDoors(bool canUp, bool canBottom, bool canRight, bool canLeft)
        {
            DoorUp.SetActive(canUp);
            DoorBottom.SetActive(canBottom);
            DoorLeft.SetActive(canLeft);
            DoorRight.SetActive(canRight);
        }
    }
}