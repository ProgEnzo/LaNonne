using System.Collections;
using Manager;
using UnityEngine;

namespace Controller
{
    public class EPCount : MonoBehaviour
    {
        [SerializeField] private int epValue;
        [SerializeField] private PlayerController playerController;
        private ScoreManager scoreManager;
        
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider2D;
        
        [Header("SoundEffect")]
        public AudioSource epAudioSource;
        public AudioClip epAudioClip;
        private void Start()
        {
            playerController = PlayerController.instance;
            scoreManager = ScoreManager.instance;
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("WallCollider"))
            {
                playerController.AddEp(epValue);
                scoreManager.AddEpScore(1);

                StartCoroutine(PlaySoundEP());
            }
        }

        private IEnumerator PlaySoundEP()
        {
            //ep sound
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
            epAudioSource.PlayOneShot(epAudioClip);
            yield return new WaitForSeconds(epAudioClip.length);
            
            Destroy(gameObject);
        }
    }
}
