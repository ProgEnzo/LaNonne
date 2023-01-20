using AI.Trash;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpTrashMobRange : MonoBehaviour
    {
        private TrashMobRange trashMobRange;

        private void Start()
        {
            trashMobRange = transform.parent.parent.GetComponent<TrashMobRange>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("WallCollider"))
            {
                trashMobRange.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
