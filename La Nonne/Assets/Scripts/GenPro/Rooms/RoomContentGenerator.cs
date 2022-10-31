using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomContentGenerator : MonoBehaviour
{

    #region Variables

        [SerializeField]
        private RoomGenerator playerRoom, defaultRoom, bossRoom, shopRoom;
    
        List<GameObject> spawnedObjects = new List<GameObject>();
    
        [SerializeField]
        private GraphTest graphTest;
    
    
        public Transform itemParent;
    
        [SerializeField]
        private CinemachineVirtualCamera cinemachineCamera;

    #endregion
    
    public void GenerateRoomContent(DungeonData dungeonData)
    {
        foreach (GameObject item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        SelectPlayerSpawnPoint(dungeonData);
        SelectBossSpawnPoints(dungeonData);
        SelectShopSpawnPoints(dungeonData);
        SelectEnemySpawnPoints(dungeonData);
        

        foreach (GameObject item in spawnedObjects)
        {
            if(item != null)
                item.transform.SetParent(itemParent, false);
        }
    }
    private void FocusCameraOnThePlayer(Transform playerTransform) //ne fonctionne pas 
    {
        cinemachineCamera.LookAt = playerTransform;
        cinemachineCamera.Follow = playerTransform;
    }

    #region PlayerRoom

    private Vector2Int playerSpawnRoomPosition;
        private void SelectPlayerSpawnPoint(DungeonData dungeonData)
        {
            int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.roomsDictionary.Count);
            Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);
    
            graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);
    
            playerSpawnRoomPosition = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);
    
            List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
                playerSpawnPoint,
                dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
                dungeonData.GetRoomFloorWithoutCorridors(playerSpawnRoomPosition)
                );
    
            FocusCameraOnThePlayer(placedPrefabs[placedPrefabs.Count - 1].transform);
    
            spawnedObjects.AddRange(placedPrefabs);
    
            dungeonData.roomsDictionary.Remove(playerSpawnPoint);
        }

    #endregion
    
    #region BossRoom
    private Vector2Int GetMapFromTilePosition(Vector2Int tilePosition, DungeonData dungeonData) //prend la position des "map" soit des salles en fonction des tiles qui les composent
        {
            Vector2Int nearthestRoom = playerSpawnRoomPosition;
            foreach (var salle in dungeonData.roomsDictionary.Keys)
            {
                if (Vector2Int.Distance(nearthestRoom, tilePosition) > Vector2Int.Distance(salle, tilePosition))
                {
                    nearthestRoom = salle;
                }
            }
    
            return nearthestRoom;
        }
    
        public Vector2Int mapBoss; //Position de ma salle de boss
        private void SelectBossSpawnPoints(DungeonData dungeonData)
        {
            Vector2Int currentFurthestRoom = playerSpawnRoomPosition;
            foreach (var room in DijkstraAlgorithm.distanceDictionary.Keys) //vérification de toutes les pos à l'intérieur
            {
                if (DijkstraAlgorithm.distanceDictionary[room] >
                    DijkstraAlgorithm.distanceDictionary[currentFurthestRoom]);
                {
                    currentFurthestRoom = room;
                }
            }
            var furthestRoom = GetMapFromTilePosition(currentFurthestRoom, dungeonData);
            mapBoss = furthestRoom; //Récupération de la position de la salle de boss pour la définir comme la salle la plus éloignée du hub par les couloirs
            spawnedObjects.AddRange(bossRoom.ProcessRoom(mapBoss, dungeonData.roomsDictionary[mapBoss], dungeonData.GetRoomFloorWithoutCorridors(mapBoss)));
            
            dungeonData.roomsDictionary.Remove(mapBoss);
        }

    #endregion
    
    #region ShopRoom
    
    public Vector2Int firstShopPosition;
    public Vector2Int lastShopPosition;
    public Vector2Int shopIntervallesPositions;
    private void SelectShopSpawnPoints(DungeonData dungeonData)
    {
        Vector2Int shopRoomPosition = playerSpawnRoomPosition;
        foreach (var shop in dungeonData.roomsDictionary.Keys) 
        {
            if (DijkstraAlgorithm.distanceDictionary [shop] == 30)
            {
                shopRoomPosition = shop;
                
                var firstShop = GetMapFromTilePosition(shopRoomPosition, dungeonData);
                firstShopPosition = firstShop;

                spawnedObjects.AddRange(shopRoom.ProcessRoom(firstShopPosition, dungeonData.roomsDictionary[firstShopPosition], dungeonData.GetRoomFloorWithoutCorridors(firstShopPosition)));
                
                dungeonData.roomsDictionary.Remove(firstShopPosition); 
                
                break;
            }
        }
        
        Vector2Int shopRoomPosition2 = playerSpawnRoomPosition;
        foreach (var shop in dungeonData.roomsDictionary.Keys) 
        {
            if (DijkstraAlgorithm.distanceDictionary [shop] == DijkstraAlgorithm.distanceDictionary[mapBoss] - 30) //30 étant la longueur des couloirs ou "corridors Length" dans l'IDE
            {
                shopRoomPosition2 = shop;
                
                var secondShop = GetMapFromTilePosition(shopRoomPosition2, dungeonData);
                lastShopPosition = secondShop;

                spawnedObjects.AddRange(shopRoom.ProcessRoom(lastShopPosition, dungeonData.roomsDictionary[lastShopPosition], dungeonData.GetRoomFloorWithoutCorridors(lastShopPosition)));
                dungeonData.roomsDictionary.Remove(lastShopPosition); 
                
                break;
            }
        }

        Vector2Int shopRoomPositionsIntervalles = firstShopPosition;
        
        while (DijkstraAlgorithm.distanceDictionary [lastShopPosition] - DijkstraAlgorithm.distanceDictionary [shopRoomPositionsIntervalles] >= 90)
        {
            shopRoomPositionsIntervalles = firstShopPosition;
            
            foreach (var shop in dungeonData.roomsDictionary.Keys)  
            {
                if (DijkstraAlgorithm.distanceDictionary [shop] == DijkstraAlgorithm.distanceDictionary[shopRoomPositionsIntervalles] + 90)
                {
                    shopRoomPositionsIntervalles = shop;
                    
                    var shopPosition = GetMapFromTilePosition(shopRoomPositionsIntervalles, dungeonData);
                    shopIntervallesPositions = shopPosition;
                    
                    spawnedObjects.AddRange(shopRoom.ProcessRoom(shopIntervallesPositions, dungeonData.roomsDictionary[shopIntervallesPositions], dungeonData.GetRoomFloorWithoutCorridors(shopIntervallesPositions)));
                    dungeonData.roomsDictionary.Remove(shopIntervallesPositions);
                }
            }
            break;
        }
        
    }

    #endregion
    
     #region DefaultRoom
        private void SelectEnemySpawnPoints(DungeonData dungeonData)
        {
            foreach (KeyValuePair<Vector2Int,HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary)
            { 
                spawnedObjects.AddRange(defaultRoom.ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
            }
        }
        
        #endregion
}
