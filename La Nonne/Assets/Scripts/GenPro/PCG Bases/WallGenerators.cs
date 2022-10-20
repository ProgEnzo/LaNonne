using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class WallGenerators 
{
  public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
  {
    var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.CardinalDirectionsList);
    var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.DiagonalDirectionsList);
    CreateBasicWall (tilemapVisualizer, basicWallPositions, floorPositions);
    CreateCornerWall (tilemapVisualizer, cornerWallPositions, floorPositions);
  }

  private static void CreateCornerWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
  {
    foreach (var position in cornerWallPositions)
    {
      string neighboursBinaryType = "";
      foreach (var direction in Direction2D.EightDirectionsList)
      {
        var neighboursPosition = position + direction;
        if (floorPositions.Contains(neighboursPosition))
        {
          neighboursBinaryType += "1";
        }
        else
        {
          neighboursBinaryType += "0";
        }
      }
      
      tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
    }
  }

  private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
  {
    foreach (var position in basicWallPositions)
    {
      string neighboursBinaryType = "";
      foreach (var direction in Direction2D.CardinalDirectionsList)
      {
        var neighboursPosition = position + direction;
        if (floorPositions.Contains(neighboursPosition))
        {
          neighboursBinaryType += "1";
        }
        else
        {
          neighboursBinaryType += "0";
        }
      }
      tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType);
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
