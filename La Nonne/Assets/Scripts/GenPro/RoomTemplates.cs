using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
   public GameObject[] bottomRooms;
   public GameObject[] topRooms;
   public GameObject[] leftRooms;
   public GameObject[] rightRooms;

   public GameObject closedRooms;
   public List<GameObject> rooms;

   public float WaitTime;
   public bool spawnedBoss;
   public GameObject boss;

   private void Update()
   {
      if (WaitTime <= 0 && spawnedBoss == false) // Allow us to spawn one unique boss and not a bunch of them
      {
         for (int i = 0; i < rooms.Count; i++)
         {
            if (i == rooms.Count - 1)
            {
               Instantiate(boss, rooms[i].transform.position, quaternion.identity);
               spawnedBoss = true;
            }
         }
      }
      else
      {
         WaitTime -= Time.deltaTime;
      }
   }
}
