using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class HealingJar : MonoBehaviour
{
    public GameObject healDrop;
    private GameObject healDropObject;
    public int numberOfHealDrops;
    private CamManager _camManager;

    public List<GameObject> healDropList = new();

    public SpriteRenderer jarSpriteRenderer;
    public SpriteRenderer jarSpriteRenderer2;
    private BoxCollider2D boxCollider2D;
    private Light2D light2DScript;
    
    [Header("SoundEffect")] 
    public AudioSource jarAudioSource;
    public AudioClip[] jarSound;

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        light2DScript = GetComponent<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Blade") || col.gameObject.CompareTag("ChainBladeHit"))
        {
            CamManager.instance.DestroyingHealthJarState(1);
            
            for (int i = 0; i < numberOfHealDrops; i++)
            {
                healDropObject = Instantiate(healDrop, transform.position, Quaternion.identity);
                
                healDropList.Add(healDropObject);

                StartCoroutine(SpriteDeactivate());
            }
            

        }
    }

    IEnumerator SpriteDeactivate()
    {
        jarSpriteRenderer.enabled = false;
        jarSpriteRenderer2.enabled = false;
        boxCollider2D.enabled = false;
        light2DScript.enabled = false;
        
        jarAudioSource.PlayOneShot(jarSound[Random.Range(0, jarSound.Length)]);
        yield return new WaitForSeconds(1f);
        
        Destroy(gameObject);
    }
}
