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
        private CircleCollider2D circleCollider2D;
        
        [Header("SoundEffect")]
        public AudioSource epAudioSource;
        public AudioClip epAudioClip;
        private void Start()
        {
            playerController = PlayerController.instance;
            scoreManager = ScoreManager.instance;
            spriteRenderer = GetComponent<SpriteRenderer>();
            circleCollider2D = GetComponent<CircleCollider2D>();
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
            circleCollider2D.enabled = false;
            epAudioSource.PlayOneShot(epAudioClip);
            epAudioSource.pitch = Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(epAudioClip.length);
            
            Destroy(gameObject);
        }
    }
}
