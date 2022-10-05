using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Guid = Pathfinding.Util.Guid;


public class CorridorsFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField] private int corridorLength = 14, corridorCount = 5; //Longueur et nombre de couloirs
    [SerializeField] [Range(0.1f, 1f)] private float roomPercent = 0.8f; //permet de générer les rooms en fin de couloirs

    protected override void RunProceduralGeneration()
    {
        CorridorsFirstDungeonGeneration();
    }

    private void CorridorsFirstDungeonGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>(); 

        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        
        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions); //to paint floor Tiles
        WallGenerators.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>(); //position we return
        int roomsToCreateToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent); //nombre de rooms à créer

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsToCreateToCreateCount).ToList(); //Potential room position in a random order / Randomly sort the HashSet

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition); //Génére des salles à la position que nous avons séléctionner au random
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition); //Start position

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength); //Générer la position des couloirs
            currentPosition = corridor[corridor.Count - 1]; //set the next current position to the last position on corridors to make sure corridors are connected
            potentialRoomPositions.Add(currentPosition); //setting the new position
            floorPositions.UnionWith(corridor);
        }
    }
    
}
