using Controller;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    [SerializeField] private float attractorSpeed = 5f;
    private bool isAttracted;
    private PlayerController playerController;

    private void Start()
    {
        playerController = PlayerController.instance;
        isAttracted = false;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("WallCollider"))
        {
            isAttracted = true;
        }
    }

    private void Update()
    {
        if (isAttracted)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerController.transform.position, attractorSpeed * Time.deltaTime);
        }
        
        if (transform.childCount < 1)
        {
            Destroy(gameObject);
        }
    }
}