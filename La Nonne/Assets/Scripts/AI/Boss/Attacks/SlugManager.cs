using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlugManager : MonoBehaviour
{
    public int numberOfBullets;
    public float bulletSpeed;

    public GameObject slugBullet;
    private void Start()
    {
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        StartCoroutine(Go());
    }

    private IEnumerator Go()
    {
        transform.DOMove(new Vector2(Random.Range(3f, 6f), Random.Range(3f, 6f)), 3f).OnComplete(GoToNextPoint);
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < numberOfBullets; i++)
        {
            var slugBulletObject = Instantiate(slugBullet, transform.position, Quaternion.identity);
            slugBulletObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(0f, 360f), Random.Range(3f, 6f)) * bulletSpeed);

        }



    }
}
