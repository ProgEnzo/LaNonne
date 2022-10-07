using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class ProceduralGenerationAlgorithms 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition); //Initialise le point de départ du chemin
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++) //pour chaque pas effectué on avance de 1 en plus
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection(); //We move one step from the start position to a random direction
            path.Add(newPosition);
            previousPosition = newPosition; //permet de recommencer à avancer
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength) //Fait que l'algo de randomWalk choisisse une seule direction et marche dans celle-ci
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);
        
        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0) //Tant qu'il y a des salles à split on lance l'algo
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (UnityEngine.Random.value < 0.5f)
                { //Verifier si on peut psplit horizontalement d'abord
                    if (room.size.y >= minHeight * 2) //if we can split the room because the space size allow it then we do it
                    {
                        SplitHorizontally(minHeight, roomsQueue, room); //We define a minimum height and with for the room we want
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) //then we cannot create a new room
                    {
                        roomsList.Add(room); //on ne la met pas dans la queue
                    }
                }
                else
                { //Vérifier si on peut split verticalement ensuite
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth,roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2) //if we can split the room because the space size allow it then we do it
                    {
                        SplitHorizontally(minHeight, roomsQueue, room); //We define a minimum height and with for the room we want
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) //then we cannot create a new room
                    {
                        roomsList.Add(room); //on ne la met pas dans la queue
                    }
                }
            }
        }

        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x); //start the random at 1 to not cut the room at the min
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y); //start the random at 1 to not cut the room at the min
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    public static List<Vector2Int> CardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //Up
        new Vector2Int(1, 0), //Right
        new Vector2Int(0, -1), //Down
        new Vector2Int(-1, 0) //Left
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return CardinalDirectionsList[Random.Range(0, CardinalDirectionsList.Count)]; //Allow us to get a random direction for the corridor
    }
}
