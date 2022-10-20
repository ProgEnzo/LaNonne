using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


public class CorridorsFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [Header("PCG Parameters")]
    [SerializeField] 
    private int corridorLength = 14, corridorCount = 5; //Longueur et nombre de couloirs
    [SerializeField]
    [Range(0.1f, 1f)] 
    private float roomPercent = 0.8f; //permet de générer les rooms en fin de couloirs
    
    [Header("PCG Data")]
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary 
        = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    
    private HashSet<Vector2Int> floorPositions, corridorPositions;

    [Header("Gizmos Data")]
    private List<Color> roomColors = new List<Color>();
    [SerializeField]
    private bool showRoomGizmo = false, showCorridorsGizmo;

    [Header("Events")]
    public UnityEvent<DungeonData> OnDungeonFloorReady;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
        DungeonData data = new DungeonData
        {
            roomsDictionary = this.roomsDictionary,
            corridorPositions = this.corridorPositions,
            floorPositions = this.floorPositions
        };
        OnDungeonFloorReady?.Invoke(data);
    }

    public void CorridorFirstGeneration()
    {
        floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>(); 

        CreateCorridors(floorPositions, potentialRoomPositions);

        //HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        //List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        //CreateRoomsAtDeadEnd(deadEnds, roomPositions);
        //floorPositions.UnionWith(roomPositions);
        //tilemapVisualizer.PaintFloorTiles(floorPositions); //to paint floor Tiles
        //WallGenerators.CreateWalls(floorPositions, tilemapVisualizer);
        
        GenerateRooms(potentialRoomPositions);
    }

    private void GenerateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerators.CreateWalls(floorPositions, tilemapVisualizer);
    }
    
    private IEnumerator GenerateRoomsCoroutine(HashSet<Vector2Int> potentialRoomPositions)
    {
        yield return new WaitForSeconds(2);
        tilemapVisualizer.Clear();
        GenerateRooms(potentialRoomPositions);
        DungeonData data = new DungeonData
        {
            roomsDictionary = this.roomsDictionary,
            corridorPositions = this.corridorPositions,
            floorPositions = this.floorPositions
        };
        OnDungeonFloorReady?.Invoke(data);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                SaveRoomData(position, room);
                roomFloors.UnionWith(room);
            }
        }
    }

    public List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPositions) 
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.CardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }

            if (neighboursCount == 1)
                deadEnds.Add(position);
        }

        return deadEnds;
    }

    public HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>(); //position we return
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent); //nombre de rooms à créer

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList(); //Potential room position in a random order / Randomly sort the HashSet
        ClearRoomData();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition); //Génére des salles à la position que nous avons séléctionner au random
            SaveRoomData(roomPosition, roomFloor);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }
    
    private void ClearRoomData()
    {
        roomsDictionary.Clear();
        roomColors.Clear();
    }

    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        roomsDictionary[roomPosition] = roomFloor;
        roomColors.Add(UnityEngine.Random.ColorHSV());
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
        corridorPositions = new HashSet<Vector2Int>(floorPositions);
    }

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        if (showRoomGizmo)
        {
            int i = 0;
            foreach (var roomData in roomsDictionary)
            {
                Color color = roomColors[i];
                color.a = 0.5f;
                Gizmos.color = color;
                Gizmos.DrawSphere((Vector2)roomData.Key, 0.5f);
                foreach (var position in roomData.Value)
                {
                    Gizmos.DrawCube((Vector2)position + new Vector2(0.5f,0.5f), Vector3.one);
                }
                i++;
            }
        }
        if (showCorridorsGizmo && corridorPositions != null)
        {
            Gizmos.color = Color.magenta;
            foreach (var corridorTile in corridorPositions)
            {
                Gizmos.DrawCube((Vector2)corridorTile + new Vector2(0.5f, 0.5f), Vector3.one);
            }
        }
    }

    #endregion
}
