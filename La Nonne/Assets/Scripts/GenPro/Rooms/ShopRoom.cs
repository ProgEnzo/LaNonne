using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : RoomGenerator
{
    public List<ItemPlacementData> itemData;

    [SerializeField]
    private PrefabPlacer prefabPlacer;
    
    public override List<GameObject> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors)
    {
        base.roomCenter = roomCenter;
        
        ItemPlacementHelper itemPlacementHelper = 
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

        List<GameObject> placedObjects = 
            prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper);

        Vector2Int shopKeeperSpawnPoint = roomCenter;

        return placedObjects;
    }
}
