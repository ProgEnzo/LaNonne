using AI.Elite;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpAutophage : MonoBehaviour
    {
        private Autophagic autophage;

        private void Start()
        {
            autophage = transform.parent.parent.GetComponent<Autophagic>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                autophage.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
