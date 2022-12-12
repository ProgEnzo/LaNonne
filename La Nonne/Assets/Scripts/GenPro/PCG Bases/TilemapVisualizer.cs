using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//[ExecuteInEditMode] //enlever quand terminé // permet d'éxécuter le script sans lancer le playmode
public class TilemapVisualizer : MonoBehaviour
{
    [Header("Tilemaps Used")] [SerializeField]
    private Tilemap
        floorTilemap,
        wallTilemap; //diférentes tilemap utilisables (Si tu veux changer de tilemepa en fonction des salles et autres tu peux)

    [Header("Tiles Used")] [SerializeField]
    private TileBase floorTile,
        wallTop,
        wallSideRight,
        wallSideLeft,
        wallBottom,
        wallFull,
        wallInnerCornerDownLeft,
        wallInnerCornerDownRight,
        wallDiagonalCornerDownRight,
        wallDiagonalCornerDownLeft,
        wallDiagonalCornerUpRight,
        wallDiagonalCornerUpLeft; //les différentes tiles utilisables

    private DungeonData dungeonData;

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
        var tilePosition = tilemap.WorldToCell((Vector3Int) position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    /// <summary>
    /// remove all tiles of a map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SwipeMap(int x, int y)
    {
        int count = 21;
        
        List<Vector2Int> tileReturn = new ();

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                var cellX = x - count / 2 + i;
                var cellY = y - count / 2 + j;
                
                wallTilemap.SetTile(new Vector3Int(cellX, cellY, 0), null);
                floorTilemap.SetTile(new Vector3Int(cellX, cellY, 0), null);
            }
        }
    }

    public bool hasCorridor(int x, int y)
    {
        return floorTilemap.GetTile( new Vector3Int( x, y)) != null;
    }
    
    /*public void Update()
    {
        var a = wallTilemap.GetTile(new Vector3Int(-8, 1, 0));
        Debug.Log(a);
    }*/

    //Appler cette fonction pour toute mes salles 
    public List<Vector2Int> GetWalls(int middleX, int middleY, PlacementType typeWanted) //surement remplacer le placement Type (actuellement mon script) 
    {
        int count = 14;
        
        List<Vector2Int> tileReturn = new ();

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                var cellX = middleX - count / 2 + i;
                var cellY = middleY - count / 2 + j;
                
                var tile = wallTilemap.GetTile(new Vector3Int(cellX, cellY, 0));

                switch (typeWanted)
                {
                    case PlacementType.WallUp:
                        if (tile == wallTop)
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                    case PlacementType.wallRight:
                        if (tile == wallSideRight)
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                    case PlacementType.wallLeft:
                        if (tile == wallSideLeft)
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                    case PlacementType.WallDown:
                        if (tile == wallBottom || tile == wallInnerCornerDownLeft || tile == wallInnerCornerDownRight)
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                }
            }
        }

        return tileReturn;
    }
    
    public List<Vector2Int> GetFloors(int middleX, int middleY)
    {
        int count = 25;
        
        List<Vector2Int> tileReturn = new ();

        for (int i = 0; i < count; i+=2)
        {
            for (int j = 0; j < count; j+=2)
            {
                var cellX = middleX - count / 2 + i;
                var cellY = middleY - count / 2 + j;
                
                var tile = floorTilemap.GetTile(new Vector3Int(cellX, cellY, 0));
                var tileX = floorTilemap.GetTile(new Vector3Int(cellX +1, cellY, 0));
                var tileY = floorTilemap.GetTile(new Vector3Int(cellX, cellY -1, 0));
                var tileXY = floorTilemap.GetTile(new Vector3Int(cellX +1, cellY -1, 0));


                if (tile == floorTile && tileX == floorTile && tileY == floorTile && tileXY == floorTile) //pas instancier dans les couloirs 
                {
                    tileReturn.Add(new Vector2Int(cellX, cellY));
                }

            }
        }

        return tileReturn;
    }
    
    public List<Vector2Int> GetFloorsNearWalls(int middleX, int middleY, PlacementType typeWanted)
    {
        int count = 25;
        
        List<Vector2Int> tileReturn = new ();

        for (int i = 0; i < count; i+=2)
        {
            for (int j = 0; j < count; j+=2)
            {
                var cellX = middleX - count / 2 + i;
                var cellY = middleY - count / 2 + j;
                
                var tile = floorTilemap.GetTile(new Vector3Int(cellX, cellY, 0)); 
                var wallTile = wallTilemap.GetTile(new Vector3Int(cellX, cellY, 0));
                
                var tileX = floorTilemap.GetTile(new Vector3Int(cellX +1, cellY, 0));
                var wallTileX = wallTilemap.GetTile(new Vector3Int(cellX +1, cellY, 0));
                var tileXBis = floorTilemap.GetTile(new Vector3Int(cellX -1, cellY, 0));
                var wallTileXBis = wallTilemap.GetTile(new Vector3Int(cellX -1, cellY, 0));
                
                var tileY = floorTilemap.GetTile(new Vector3Int(cellX, cellY -1, 0));
                var wallTileY = wallTilemap.GetTile(new Vector3Int(cellX, cellY -1, 0));
                var tileYBis = floorTilemap.GetTile(new Vector3Int(cellX, cellY +1, 0));
                var wallTileYBis = wallTilemap.GetTile(new Vector3Int(cellX, cellY +1, 0));
                
                var tileXY = floorTilemap.GetTile(new Vector3Int(cellX +1, cellY -1, 0));
                var wallTileXY = wallTilemap.GetTile(new Vector3Int(cellX +1, cellY -1, 0));
                var tileXYBis = floorTilemap.GetTile(new Vector3Int(cellX -1, cellY +1, 0));
                var wallTileXYBis = wallTilemap.GetTile(new Vector3Int(cellX -1, cellY +1, 0));

                switch (typeWanted)
                {
                    case PlacementType.nearWallUp :
                        if (wallTile == wallTop && tileY == floorTile) //pas instancier dans les couloirs 
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                    case PlacementType.nearWallRight :
                        if (wallTile == wallSideRight && tileXBis == floorTile) //pas instancier dans les couloirs 
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                    case PlacementType.nearWallDown :
                        if (wallTile == wallBottom && tileYBis == floorTile)
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                    case PlacementType.nearWallLeft :
                        if (wallTile == wallSideLeft && tileX == floorTile)
                        {
                            tileReturn.Add(new Vector2Int(cellX, cellY));
                        }
                        break;
                }
            }
        }

        return tileReturn;
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
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottomEightDirections.Contains(typeAsInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }
}