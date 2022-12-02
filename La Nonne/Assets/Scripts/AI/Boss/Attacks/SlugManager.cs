using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlugManager : MonoBehaviour
{
    private PlayerController player;
    public GameObject slugBullet;

    public Rigidbody2D rb;

    public int numberOfBullets;
    public float bulletSpeed;
    public int dashAmount;
    public float massReduction;
    public float bulletMassReduction;
    public float dashSpeed;

    private void Start()
    {
        player = PlayerController.instance;
        rb = GetComponent<Rigidbody2D>();

        GoToNextPoint();
    }
    void GoToNextPoint()
    {
        StartCoroutine(Go());
    }

    private IEnumerator Go()
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f)); //Randomize
        
        dashAmount--;
        bulletMassReduction += 0.2f;
        var directionDash = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0) - transform.position.normalized; //Direction Dash
        rb.velocity = directionDash * dashSpeed; //Dash Slug
        yield return new WaitForSeconds(Random.Range(0f, 3f)); //Randomize

        var localScaleProj = slugBullet.transform.localScale;

        for (int i = 0; i < numberOfBullets; i++)
        {
            var slugBulletObject = Instantiate(slugBullet, transform.position, Quaternion.identity); //spawn slugBullet
            slugBulletObject.transform.localScale += localScaleProj - new Vector3(bulletMassReduction, bulletMassReduction, 0); //reduction masse bullet
            
            var direction = player.transform.position - transform.position;
            
            slugBulletObject.GetComponent<Rigidbody2D>().AddForce(direction * new Vector2(Random.Range(-5f, 10f), Random.Range(-5f, 10f)) * bulletSpeed); //bullet goes to player
            Destroy(slugBulletObject, 5f);
        }

        transform.DOScale(new Vector2(transform.localScale.x - massReduction, transform.localScale.y - massReduction), 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);

        if (dashAmount > 0)
        {
            StartCoroutine(Go());
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
