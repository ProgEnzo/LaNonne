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

    public int numberOfBullets;
    public float bulletSpeed;
    public int dashAmount;
    public float massReduction;
    public float bulletMassReduction;

    private void Start()
    {
        player = PlayerController.instance;

        GoToNextPoint();
    }
    void GoToNextPoint()
    {
        StartCoroutine(Go());
    }

    private IEnumerator Go()
    {
        dashAmount--;
        bulletMassReduction += 0.2f;
        var directionDash = player.transform.position - transform.position;
        
        //NE FONCTIONNE PAS WITH OTHER COLLIDERS
        transform.DOMove(directionDash, 3f);
        yield return new WaitForSeconds(3f);

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
