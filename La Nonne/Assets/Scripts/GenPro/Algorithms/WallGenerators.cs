using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerators 
{
  public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
  {
    var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.CardinalDirectionsList);
    foreach (var positions in basicWallPositions)
    {
      tilemapVisualizer.PaintSingleBasicWall(positions);
    }
  }

  private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
  {
    HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
    foreach (var positions in floorPositions)
    {
      foreach (var direction in directionList)
      {
        var neighbourPosition = positions + direction;
        if (floorPositions.Contains(neighbourPosition) == false)
        {
          wallPositions.Add(neighbourPosition);
        }
      }
    }

    return wallPositions;
  }
}
