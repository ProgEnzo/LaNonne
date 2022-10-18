using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Controller;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
   [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4; //Taille minimale des salles apres division / split
   [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20; //tailles des salles que l'on split dans le BSP algo
   [SerializeField] [Range(0, 10)] private int offSet = 1; //To not directly connect every rooms
   
   [SerializeField] private bool randomWalkRooms = false;
   
   public UnityEvent OnFinishedRoomGeneration;

   private static DungeonData dungeonData;

   private void Awake()
   {
      dungeonData = FindObjectOfType<DungeonData>();
      if (dungeonData == null)
         dungeonData = gameObject.AddComponent<DungeonData>();
   }
   
   public static void Generate()
   {
      dungeonData.Reset();
      
      //dungeonData.Rooms.Add(new Room(new Vector2Int(0, 0), new HashSet<Vector2Int>()));
   }

   protected override void RunProceduralGeneration()
   {
      CreateRooms();
   }
   
   public List<Vector2Int> roomCenters = new List<Vector2Int>(); //Creating rooms first

   public void CreateRooms()
   {
      var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
         new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
         minRoomHeight);

      HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

      if (randomWalkRooms)
      {
         floor = CreateRoomsRandomly(roomsList);
      }
      else
      {
         floor = CreateSimpleRooms(roomsList);
      }
      
      //List<Vector2Int> roomCenters = new List<Vector2Int>(); //Creating romms first

      foreach (var room in roomsList)
      {
          roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
          dungeonData.Rooms.Add(new Room(room.center, floor)); //A voir si ca marche (je ne suis pas sur)
          OnFinishedRoomGeneration?.Invoke();
      }

      if(PlayerController.instance is not null)
      {
         PlayerController.instance.ReInit();
      }

      HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
      floor.UnionWith(corridors); //to spawn floor tiles for corridors as well

      tilemapVisualizer.PaintFloorTiles(floor);
      WallGenerators.CreateWalls(floor, tilemapVisualizer);
   }

   /*public Vector2Int PlayerSpawnPosition()
   {
      roomCenters
   }*/
   
   private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
   {
      HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
      for (int i = 0; i < roomsList.Count; i++)
      {
         var roomBounds = roomsList[i];
         var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
         var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
         foreach (var position in roomFloor)
         {
            if (position.x >= (roomBounds.xMin + offSet) && position.x <= (roomBounds.xMax - offSet) &&
                position.y >= (roomBounds.yMin - offSet) && position.y <= (roomBounds.yMax - offSet))
            {
               floor.Add(position);
            }
         }

      }

      return floor;
   }

   public HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
   {
      HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
      var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
      roomCenters.Remove(currentRoomCenter); //remove the current center of the room to not use it anymore and not connect plenty corridors to it

      while (roomCenters.Count > 0) //tant qu'il y a encore des salles avec le centre inutilisé
      {
         Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters); //on récupére le centre
         roomCenters.Remove(closest); //on retire celui-ci de la liste
         HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
         currentRoomCenter = closest; 
         corridors.UnionWith(newCorridor);
      }

      return corridors;
   }

   private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
   {
      HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
      var position = currentRoomCenter;
      corridor.Add(position);
      while (position.y != destination.y)
      {
         if (destination.y > position.y)
         {
            position += Vector2Int.up;
         }
         else if (destination.y < position.y)
         {
            position += Vector2Int.down;
         }

         corridor.Add(position);
      }

      while (position.x != destination.x)
      {
         if (destination.x > position.x)
         {
            position += Vector2Int.right;
         }
         else if (destination.x < position.x)
         {
            position += Vector2Int.left;
         }

         corridor.Add(position);
      }

      return corridor;
   }

   private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
   {
      Vector2Int closest = Vector2Int.zero;
      float distance = float.MaxValue;
      foreach (var position in roomCenters)
      {
         float currentDistance = Vector2.Distance(position, currentRoomCenter);
         if (currentDistance < distance)
         {
            distance = currentDistance;
            closest = position;
         }
      }

      return closest;
   }

   private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
   {
      HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
      foreach (var room in roomsList)
      {
         for (int col = offSet; col < room.size.x - offSet; col++)
         {
            for (int row = offSet; row < room.size.y - offSet; row++)
            {
               Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
               floor.Add(position);
            }
         }
      }

      return floor;
   }
}
