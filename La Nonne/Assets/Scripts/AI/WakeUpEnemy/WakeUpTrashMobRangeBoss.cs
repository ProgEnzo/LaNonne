using System.Collections;
using AI.Trash;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpTrashMobRangeBoss : MonoBehaviour
    {
        private TrashMobRange trashMobRange;
        private Rigidbody2D rb;
        [SerializeField] private GameObject enemyPuppet;
        [SerializeField] private GameObject chrysalisPuppet;

        private void Start()
        {
            var parent = transform.parent;
            trashMobRange = parent.GetComponent<TrashMobRange>();
            rb = parent.GetComponent<Rigidbody2D>();
            StartCoroutine(Activation());
        }
        
        private IEnumerator Activation()
        {
            yield return new WaitForSeconds(2f);
            enemyPuppet.SetActive(true);
            chrysalisPuppet.SetActive(false);
            trashMobRange.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            Destroy(gameObject);
        }
    }
}
