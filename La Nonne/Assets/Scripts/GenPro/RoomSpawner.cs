using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSpawner : MonoBehaviour
{
   public int openingDirection;

   private RoomTemplates templates;
   private int rand;
   private bool spawned = false;

   private void Start()
   {
      templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
      Invoke("Spawn", 2f);
   }

   private void Spawn()
   {
      if (spawned == false)
      {
         if (openingDirection == 1)
         {
            var rand = Random.Range(0, templates.bottomRooms.Length);
            Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
         }
         else if (openingDirection == 2)
         {
            var rand = Random.Range(0, templates.topRooms.Length);
            Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
         }
         else if (openingDirection == 3)
         {
            var rand = Random.Range(0, templates.leftRooms.Length);
            Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
         }
         else if (openingDirection == 4)
         {
            var rand = Random.Range(0, templates.rightRooms.Length);
            Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
         }

         spawned = true;   
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("SpawnPoint"))
      {
         Destroy(gameObject);
      }
   }
}
