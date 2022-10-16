using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
   [Header("Tilemaps Used")]
   [SerializeField] private Tilemap floorTilemap, wallTilemap; //diférentes tilemap utilisables (Si tu veux changer de tilemepa en fonction des salles et autres tu peux)

   [Header("Tiles Used")]
   [SerializeField] private TileBase floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull, wallInnerCornerDownLeft, 
      wallInnerCornerDownRight, wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, 
      wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft; //les différentes tiles utilisables

   public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
   {
      PaintTiles(floorPositions, floorTilemap, floorTile);
   }
   
   public void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
   {
      foreach (var position in positions)
      {
         PaintSingleTile(tilemap, tile, position);
      }
   }

   private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
   {
      var tilePosition = tilemap.WorldToCell((Vector3Int)position);
      tilemap.SetTile(tilePosition, tile);
   }

   public void Clear()
   {
      floorTilemap.ClearAllTiles();
      wallTilemap.ClearAllTiles();
   }

   public void PaintSingleBasicWall(Vector2Int positions, string binaryType)
   {
      int typeAsInt = Convert.ToInt32(binaryType, 2); //Converti le string de la valeur bianire en valeur décimale
      TileBase tile = null;
      if (WallTypesHelper.wallTop.Contains(typeAsInt))
      {
         tile = wallTop;
      }
      else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
      {
         tile = wallSideRight;
      }
      else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
      {
         tile = wallSideLeft;
      }
      else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
      {
         tile = wallBottom;
      }
      else if (WallTypesHelper.wallFull.Contains(typeAsInt))
      {
         tile = wallFull;
      }
      if (tile != null)
         PaintSingleTile(wallTilemap, tile, positions);
      
      //PaintSingleTile(wallTilemap, wallTop, positions); //walltilemap référe à la tilemap qu'on utilise pour peindre, walltop référe à quelle tile on utilise et position référe à l'endroit où on peint (position est dséfini dans un autre script)
   }

   public void PaintSingleCornerWall(Vector2Int position, string binaryType)
   {
      int typeAsInt = Convert.ToInt32(binaryType, 2);
      TileBase tile = null;
      if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
      {
         tile = wallInnerCornerDownLeft;
      }
      else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
      {
         tile = wallInnerCornerDownRight;
      }
      else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
      {
         tile = wallDiagonalCornerDownRight;
      }
      else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
      {
         tile = wallDiagonalCornerDownLeft;
      }
      else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
      {
         tile = wallDiagonalCornerUpRight;
      }
      else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
      {
         tile = wallDiagonalCornerUpLeft;
      }
      else if(WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
      {
         tile = wallFull;
      }
      else if(WallTypesHelper.wallBottomEightDirections.Contains(typeAsInt))
      {
         tile = wallBottom;
      }
      
      if (tile != null)
         PaintSingleTile(wallTilemap, tile, position);
   }
}
