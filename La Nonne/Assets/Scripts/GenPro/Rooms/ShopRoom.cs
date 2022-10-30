using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : RoomGenerator
{
    public GameObject shopKeeper;
    
    public List<ItemPlacementData> itemData;

    [SerializeField]
    private PrefabPlacer prefabPlacer;
    
    public override List<GameObject> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors)
    {
        ItemPlacementHelper itemPlacementHelper = 
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

        List<GameObject> placedObjects = 
            prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper);

        Vector2Int shopKeeperSpawnPoint = roomCenter;

        GameObject playerObject 
            = prefabPlacer.CreateObject(shopKeeper, shopKeeperSpawnPoint + new Vector2(0.5f, 0.5f));
 
        placedObjects.Add(playerObject);

        return placedObjects;
    }
}
