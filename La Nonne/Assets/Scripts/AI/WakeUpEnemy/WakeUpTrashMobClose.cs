using AI.Trash;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpTrashMobClose : MonoBehaviour
    {
        private TrashMobClose trashMobClose;

        private void Start()
        {
            trashMobClose = transform.parent.parent.GetComponent<TrashMobClose>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                trashMobClose.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
