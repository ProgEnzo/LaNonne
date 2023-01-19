using AI.Elite;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpPyromaniac : MonoBehaviour
    {
        private Pyromaniac pyromaniac;

        private void Start()
        {
            pyromaniac = transform.parent.parent.GetComponent<Pyromaniac>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                pyromaniac.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
