using System.Collections;
using AI.Trash;
using UnityEngine;

namespace AI.WakeUpEnemy
{
    public class WakeUpTrashMobCloseBoss : MonoBehaviour
    {
        private TrashMobClose trashMobClose;
        private Rigidbody2D rb;
        [SerializeField] private GameObject enemyPuppet;
        [SerializeField] private GameObject chrysalisPuppet;
        [SerializeField] private GameObject stacks;

        private void Start()
        {
            var parent = transform.parent;
            trashMobClose = parent.GetComponent<TrashMobClose>();
            rb = parent.GetComponent<Rigidbody2D>();
            StartCoroutine(Activation());
        }
        
        private IEnumerator Activation()
        {
            yield return new WaitForSeconds(2f);
            enemyPuppet.SetActive(true);
            chrysalisPuppet.SetActive(false);
            stacks.SetActive(true);
            trashMobClose.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            Destroy(gameObject);
        }
    }
}
