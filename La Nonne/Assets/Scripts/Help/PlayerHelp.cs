using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHelp : MonoBehaviour
{
    public Transform launchPos;
    public GameObject epHelp;

    private RoomContentGenerator roomContentGenerator;
    private Rigidbody2D rb;

    public float speedForBouboule = 3f;
    public float timeToDestroyBouboule = 2f;
    
    public float coolDownBeforeNextBouboule = 4f;
    public float currentTimeBeforeNextBouboule;

    public void OnEnable()
    {
        roomContentGenerator = GameObject.Find("RoomContentGenerator").GetComponent<RoomContentGenerator>();
        currentTimeBeforeNextBouboule = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentTimeBeforeNextBouboule <= 0f)
        {
            rb = Instantiate(epHelp, launchPos.position, transform.rotation).GetComponent<Rigidbody2D>();
            rb.velocity = (roomContentGenerator.mapBoss - (Vector2)transform.position).normalized * speedForBouboule;
            StartCoroutine(DetruitLaBouboule());
            currentTimeBeforeNextBouboule = coolDownBeforeNextBouboule;
        }
        currentTimeBeforeNextBouboule -= Time.deltaTime;
    }

    private IEnumerator DetruitLaBouboule()
    {
        yield return new WaitForSeconds(timeToDestroyBouboule);
        Destroy(rb.gameObject);
    }
    
    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDownBeforeNextBouboule);
    }
}