using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : RoomGenerator
{
    public GameObject boss;
    
    [SerializeField]
    private PrefabPlacer prefabPlacer;
    
    public List<ItemPlacementData> itemData;
    
    public override List<GameObject> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors)
    {
        ItemPlacementHelper itemPlacementHelper =
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

        List<GameObject> placedObjects =
            prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper);
        
        Vector2Int bossSpawnPoint = roomCenter;
        
        GameObject BossObject =  prefabPlacer.CreateObject(boss, bossSpawnPoint + new Vector2(0.5f, 0.5f));
        
        placedObjects.Add(BossObject);

        return placedObjects;
    }
}
