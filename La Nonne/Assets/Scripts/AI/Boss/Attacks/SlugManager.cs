using System;
using System.Collections;
using Controller;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Boss.Attacks
{
    public class SlugManager : MonoBehaviour
    {
        private PlayerController player;
        public GameObject slugBullet;

        public Rigidbody2D rb;
        private GameObject currentSlugPuppet;

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
            
            currentSlugPuppet = transform.GetChild(Random.Range(0, transform.childCount)).gameObject;
            currentSlugPuppet.SetActive(true);
            GoToNextPoint();
        }

        private void Update()
        {
            CheckDirection();
        }

        private void GoToNextPoint()
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

            for (var i = 0; i < numberOfBullets; i++)
            {
                var slugBulletObject = Instantiate(slugBullet, transform.position, Quaternion.identity); //spawn slugBullet
                var playerDirection = player.transform.position - transform.position;
                var direction = playerDirection * new Vector2(Random.Range(-5f, 10f), Random.Range(-5f, 10f)) * bulletSpeed;
                
                slugBulletObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized) * Quaternion.Euler(0, 0, -95);

                slugBulletObject.transform.localScale += localScaleProj - new Vector3(bulletMassReduction / 2f, bulletMassReduction / 2f, 0); //reduction masse bullet
            
                slugBulletObject.GetComponent<Rigidbody2D>().AddForce(direction); //bullet goes to player
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
        
        private void CheckDirection()
        {
            var puppetLocalScale = currentSlugPuppet.transform.localScale;
            if (rb.velocity.x != 0)
            {
                currentSlugPuppet.transform.localScale = new Vector3(MathF.Sign(rb.velocity.x) * MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
            else
            {
                currentSlugPuppet.transform.localScale = player.transform.position.x > transform.position.x ? new Vector3(MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z) : new Vector3(-MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
        }
    }
}
