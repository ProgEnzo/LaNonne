using System;
using Controller;
using UnityEngine;

namespace AI
{
    public class Activer : MonoBehaviour
    {
        [SerializeField] private GameObject activationParent;
        private const int DistanceActivation = 35;
        private int modulo = 0;
        private void FixedUpdate()
        {
            modulo++;
            
            if (modulo % 15 != 0)
            {
                return;
            }
            
            activationParent.SetActive(Vector2.Distance(transform.position, PlayerController.instance.transform.position) < DistanceActivation);
        }
    }
}