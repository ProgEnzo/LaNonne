using AI.Elite;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpTDI : MonoBehaviour
    {
        private TDI tdi;

        private void Start()
        {
            tdi = transform.parent.parent.GetComponent<TDI>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("WallCollider"))
            {
                tdi.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
