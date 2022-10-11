using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
   [Header("Tilemaps Used")]
   [SerializeField] private Tilemap floorTilemap, wallTilemap; //diférentes tilemap utilisables (Si tu veux changer de tilemepa en fonction des salles et autres tu peux)

   [Header("Tiles Used")]
   [SerializeField] private TileBase floorTile, wallTop;

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

   public void PaintSingleBasicWall(Vector2Int positions)
   {
      PaintSingleTile(wallTilemap, wallTop, positions); //walltilemap référe à la tilemap qu'on utilise pour peindre, walltop référe à quelle tile on utilise et position référe à l'endroit où on peint (position est dséfini dans un autre script)
   }
}
