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
