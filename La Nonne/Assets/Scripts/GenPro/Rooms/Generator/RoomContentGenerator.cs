using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using GenPro.Rooms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class RoomContentGenerator : MonoBehaviour
{

    #region Variables

        [SerializeField]
        private RoomGenerator playerRoom, bossRoom, shopRoom, preBossRoom; //généraliser lvl0

        [SerializeField] private TilemapVisualizer tilemapVisualizer;
    
    
        List<GameObject> spawnedObjects = new List<GameObject>();
    
        [SerializeField]
        private GraphTest graphTest;
    
    
        public Transform itemParent;
    
        [SerializeField]
        private CinemachineVirtualCamera cinemachineCamera;

        [SerializeField,Space, Header("Prefab Shop")]
        private GameObject prefabShop;
        
        [SerializeField,Space, Header("Prefab Boss")]
        private GameObject prefabBoss;

        
        List<Vector2Int> shopRoomPos = new();
        
        [SerializeField] private List<float> multiplierPerDistance = new(){20, 45, 54, 67, 100};
        
        [SerializeField] private List<GameObject> wallUpCusto = new();
        [SerializeField] private List<GameObject> wallDownCusto = new();
        [SerializeField] private List<GameObject> wallRightCusto = new();
        [SerializeField] private List<GameObject> wallLeftCusto = new();
        [SerializeField] private List<GameObject> floorCusto = new();
        [SerializeField] private List<GameObject> floorNearWallsCusto = new();

        #endregion
        
    private IEnumerator Scan()
    {
        yield return new WaitForSeconds(0.4f);
        AstarPath.active.Scan();
    }
    
    Dictionary<Vector2Int, HashSet<Vector2Int>> copiedDico;
    
    
    public void GenerateRoomContent(DungeonData dungeonData)
    { 
        copiedDico = new Dictionary<Vector2Int, HashSet<Vector2Int>>(dungeonData.roomsDictionary);
        
        foreach (GameObject item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        SelectPlayerSpawnPoint(dungeonData);
        SelectBossSpawnPoints(dungeonData);
        SelectShopSpawnPoints(dungeonData);
        PreBossRoomPosition(dungeonData);
        SelectEnemySpawnPoints(dungeonData);

        ModifyBossRoom();
        ModifyShopsRoom();
                
        foreach (var roomPos in copiedDico.Keys)
        {
            PopulateWallUp(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.WallUp)); //maybe les appeler que dans mes salle pour que ca n'apparaisse que sur les murs de certaines salles
            PopulateWallDown(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.WallDown));
            PopulateWallRight(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.wallRight));
            PopulateWallLeft(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.wallLeft));
            PopulateFloor(tilemapVisualizer.GetFloors(roomPos.x, roomPos.y)); //appeler ca dans la fonction qui fait aparaitre (playerRoom, etc....) les salles fait que l'on peux moduler pour chaque salle les objets qui apparaisse sur le sol
            PopulateFloorNearWalls(tilemapVisualizer.GetFloorsNearWalls(roomPos.x, roomPos.y));
        }
        
        StartCoroutine(Scan());

        foreach (GameObject item in spawnedObjects)
        {
            if(item != null)
                item.transform.SetParent(itemParent, false);
        }
    }

    private void ModifyBossRoom()
    {
        tilemapVisualizer.SwipeMap(bossRoom.roomCenter.x, bossRoom.roomCenter.y);
        InstantiateBossRoom(bossRoom.roomCenter.x, bossRoom.roomCenter.y);
        
    }

    private void InstantiateBossRoom(int x, int y)
    {
        var go = Instantiate(prefabBoss);
        Vector2Int pos = new Vector2Int(x, y);
        go.transform.position = new Vector3(x, y);
        var detector = go.GetComponent<DoorDetector>();
        detector.ManageDoors(hasTopMap(pos),hasBottomMap(pos), hasRightMap(pos),hasLeftMap(pos));
        
    }
    
    private void ModifyShopsRoom()
    {
        foreach (var shops in shopRoomPos)
        {
            tilemapVisualizer.SwipeMap(shops.x, shops.y);
            InstantiateShop(shops.x, shops.y);
        }
    }

    private bool hasLeftMap(Vector2Int from)
    {
        return tilemapVisualizer.hasCorridor(from.x - 12, from.y);
    }
    
    private bool hasRightMap(Vector2Int from)
    {
        return tilemapVisualizer.hasCorridor(from.x + 12, from.y);
    }

    private bool hasTopMap(Vector2Int from)
    {
  
        return tilemapVisualizer.hasCorridor(from.x, from.y+12);
    }
    
    private bool hasBottomMap(Vector2Int from)
    {

        return tilemapVisualizer.hasCorridor(from.x, from.y-12);
    }
    
    
    private void InstantiateShop(int x, int y)
    {
        var go = Instantiate(prefabShop);
        Vector2Int pos = new Vector2Int(x, y);
        go.transform.position = new Vector3(x, y);
        var detector = go.GetComponent<DoorDetector>();
        detector.ManageDoors(hasTopMap(pos),hasBottomMap(pos), hasRightMap(pos),hasLeftMap(pos));
    }

    private void PopulateWallUp (List<Vector2Int> wallUpPos)
    {
        foreach (var pos in wallUpPos)
        {
            //if (Random.Range(0, 100) < 10)
            {
                GameObject item = Instantiate(wallUpCusto[Random.Range(0, wallUpCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    private void PopulateWallDown (List<Vector2Int> wallDownPos)
    {
        foreach (var pos in wallDownPos)
        {
            //if (Random.Range(0, 100) < 10)
            {
                GameObject item = Instantiate(wallDownCusto[Random.Range(0, wallDownCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    private void PopulateWallRight (List<Vector2Int> wallRightPos)
    {
        foreach (var pos in wallRightPos)
        {
            //if (Random.Range(0, 100) < 10)
            {
                GameObject item = Instantiate(wallRightCusto[Random.Range(0, wallRightCusto.Count)], new Vector3(pos.x, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    private void PopulateWallLeft (List<Vector2Int> wallLeftPos)
    {
        foreach (var pos in wallLeftPos)
        {
            //if (Random.Range(0, 100) < 10)
            {
                GameObject item = Instantiate(wallLeftCusto[Random.Range(0, wallLeftCusto.Count)], new Vector3(pos.x +1f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    private void PopulateFloor (List<Vector2Int> floorPos)
    {
        foreach (var pos in floorPos)
        {
            if (Random.Range(0, 100) < 10)
            {
                GameObject item = Instantiate(floorCusto[Random.Range(0, floorCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    private void PopulateFloorNearWalls (List<Vector2Int> floorPos)
    {
        foreach (var pos in floorPos)
        {
            //if (Random.Range(0, 100) < 10)
            {
                GameObject item = Instantiate(floorNearWallsCusto[Random.Range(0, floorNearWallsCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
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
            //int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.roomsDictionary.Count);
            //int firstRoom = dungeonData.roomsDictionary.Keys.First(); //je peux pas faire ca c'est sensé etre un vector2Int et pas un int
            
            Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.First(); //fait spawn le hub au premier élément du Dico                          //.ElementAt(randomRoomIndex); 
    
            graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);
    
            playerSpawnRoomPosition = dungeonData.roomsDictionary.Keys.First();
    
            List<GameObject> placedPrefabs = playerRoom.ProcessRoom(playerSpawnPoint, dungeonData.roomsDictionary.Values.First(), dungeonData.GetRoomFloorWithoutCorridors(playerSpawnRoomPosition));
    
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
    
    int GetDistanceWithNearestShop (Vector2Int currentPos) //Prend la distance entre les shop afin de définir le shop le plus proche 
    {
        int distance = 99999;
            
        foreach (var shopRoom in shopRoomPos)
        {
            if (Vector2Int.Distance(currentPos, shopRoom) < distance)
            {
                distance = (int)Vector2Int.Distance(currentPos, shopRoom);
            }
        }

        return distance;
    }
    
    #region ShopRoom
    
    public Vector2Int firstShopPosition;
    public Vector2Int lastShopPosition;
    private void SelectShopSpawnPoints(DungeonData dungeonData)
    {
        Vector2Int shopRoomPosition = playerSpawnRoomPosition;
        foreach (var shop in dungeonData.roomsDictionary.Keys) 
        {
            if (DijkstraAlgorithm.distanceDictionary [shop] == 22)
            {
                shopRoomPosition = shop;
                
                var firstShop = GetMapFromTilePosition(shopRoomPosition, dungeonData);
                firstShopPosition = firstShop;

                spawnedObjects.AddRange(shopRoom.ProcessRoom(firstShopPosition, dungeonData.roomsDictionary[firstShopPosition], dungeonData.GetRoomFloorWithoutCorridors(firstShopPosition)));
                
                dungeonData.roomsDictionary.Remove(firstShopPosition); 
                shopRoomPos.Add(firstShopPosition);

                
                break;
            }
        }
        
        Vector2Int shopRoomPosition2 = playerSpawnRoomPosition;
        foreach (var shop in dungeonData.roomsDictionary.Keys) 
        {
            if (DijkstraAlgorithm.distanceDictionary [shop] == DijkstraAlgorithm.distanceDictionary[mapBoss] - 44) //22 étant la longueur des couloirs ou "corridors Length" dans l'IDE
            {
                shopRoomPosition2 = shop;
                
                var secondShop = GetMapFromTilePosition(shopRoomPosition2, dungeonData);
                lastShopPosition = secondShop;

                spawnedObjects.AddRange(shopRoom.ProcessRoom(lastShopPosition, dungeonData.roomsDictionary[lastShopPosition], dungeonData.GetRoomFloorWithoutCorridors(lastShopPosition)));
                dungeonData.roomsDictionary.Remove(lastShopPosition); 
                shopRoomPos.Add(lastShopPosition);
                
                break;
            }
        }
        
        var tempDico = new Dictionary<Vector2Int, HashSet<Vector2Int>>(dungeonData.roomsDictionary); //pour faire un dico temporaire pour modif sans tout casser // permet de faire spawn les ennemis 
        
        bool hasShop = false;
        do
        {
            hasShop = false;
            
            foreach (var shop in tempDico.Keys)
            {
                int distance = GetDistanceWithNearestShop(shop);

                if (distance <= 22)
                {
                    var shopRoomPositionInter = GetMapFromTilePosition(shop, dungeonData);
                    tempDico.Remove(shopRoomPositionInter);
                    hasShop = true;
                    break;
                }

                int mapDistance = (distance - 22) / 22;
                
                float ratio = multiplierPerDistance.Count > mapDistance ? multiplierPerDistance[mapDistance] : multiplierPerDistance[^1];

                if (Random.Range(0, 100) > ratio)
                {
                    var shopRoomPositionInter = GetMapFromTilePosition(shop, dungeonData);
                    tempDico.Remove(shopRoomPositionInter);
                    hasShop = true;
                    break;
                }
                
                if (GetDistanceWithNearestShop(shop) > 22)
                {
                    var shopRoomPositionInter = GetMapFromTilePosition(shop, dungeonData);
                    spawnedObjects.AddRange(shopRoom.ProcessRoom(shopRoomPositionInter, dungeonData.roomsDictionary[shopRoomPositionInter], dungeonData.GetRoomFloorWithoutCorridors(shopRoomPositionInter)));
                            
                    tempDico.Remove(shopRoomPositionInter); //remove dans le temporaire quand c'est une fake room
                    dungeonData.roomsDictionary.Remove(shopRoomPositionInter); //remove dans le dico partagé quand c est une true room
                    shopRoomPos.Add(shopRoomPositionInter);

                    hasShop = true;
                    break;
                }
            
            }
        } while (hasShop);
    }

    #endregion
    
     #region DefaultRoom 
     
     [SerializeField] private List<EnemyRoom> enemyRoom;
         private void SelectEnemySpawnPoints(DungeonData dungeonData) //Ca fonctionne mais le fait que ce soit la distance à vol d'oiseau du hub est domageable ! Faire un A* serait trop compliqué et pas worth en fonction de la charge de taff demandée.
        {
            foreach (KeyValuePair<Vector2Int,HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary) //roomData n'est pas bon
            {
                var distance = Vector2Int.Distance(playerSpawnRoomPosition, roomData.Key) / 22; // room data bonne clés de distance ? Actuellement me donne la distance en ne passant pas par les couloirs

                if (distance <= 1) // Game Tutorial
                    spawnedObjects.AddRange(enemyRoom[0].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance < 2) 
                    spawnedObjects.AddRange(enemyRoom[1].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance < 4)  //Pyro Tutorial
                    spawnedObjects.AddRange(enemyRoom[2].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance < 5) 
                    spawnedObjects.AddRange(enemyRoom[3].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance < 7) 
                    spawnedObjects.AddRange(enemyRoom[4].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance < 8) //TDI Tutorial
                    spawnedObjects.AddRange(enemyRoom[5].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance < 10) 
                    spawnedObjects.AddRange(enemyRoom[6].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance > 13) 
                    spawnedObjects.AddRange(enemyRoom[7].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
            }
        }
        
        #endregion
        
        #region PreBossRoom
        
        private void PreBossRoomPosition(DungeonData dungeonData)
        {
            Vector2Int preBossRoomPosition = playerSpawnRoomPosition;
            
            foreach (var preBoss in dungeonData.roomsDictionary.Keys)
            {
                if (DijkstraAlgorithm.distanceDictionary[preBoss] == DijkstraAlgorithm.distanceDictionary[mapBoss] - 22)
                {
                    preBossRoomPosition = preBoss;
                    
                    var preBossPos = GetMapFromTilePosition(preBossRoomPosition, dungeonData);

                    spawnedObjects.AddRange(preBossRoom.ProcessRoom(preBossPos, dungeonData.roomsDictionary[preBossPos], dungeonData.GetRoomFloorWithoutCorridors(preBossPos)));
                    dungeonData.roomsDictionary.Remove(preBossPos);

                    break;
                }
            }
        }
        
        #endregion
     
     /*#region EnemiesRooms

     public Vector2Int firstEnemiesRoomsPosition;
     public Vector2Int secondEnemiesRoomsPosition;
     public Vector2Int thirdEnemiesRoomsPosition;
     public Vector2Int lastEnemiesRoomsPosition;
     
     bool hasEnemiesLvl0Rooms = false;
     bool hasEnemiesLvl1Rooms = false;
     bool hasEnemiesLvl2Rooms = false;
     bool hasEnemiesLvl3Rooms = false;
     
     private void EnemiesRoomLevelOne(DungeonData dungeonData)
     {
         Vector2Int enemiesLvl0RoomPosition = playerSpawnRoomPosition;
         
         do
         {
             hasEnemiesLvl0Rooms = false;
             
             foreach (var ennemiesRoomPosition in dungeonData.roomsDictionary.Keys)
             {
                 if (DijkstraAlgorithm.distanceDictionary[enemiesLvl0RoomPosition] > DijkstraAlgorithm.distanceDictionary[mapBoss] / 4)
                 {
                     enemiesLvl0RoomPosition = ennemiesRoomPosition;
                                               
                     var ennemiesLvl0 = GetMapFromTilePosition(enemiesLvl0RoomPosition, dungeonData);
                     firstEnemiesRoomsPosition = ennemiesLvl0;
                                               
                     spawnedObjects.AddRange(enemiesLvl0Room.ProcessRoom(firstEnemiesRoomsPosition, dungeonData.roomsDictionary[firstEnemiesRoomsPosition], dungeonData.GetRoomFloorWithoutCorridors(firstEnemiesRoomsPosition)));
                                               
                     dungeonData.roomsDictionary.Remove(firstEnemiesRoomsPosition);
                     hasEnemiesLvl0Rooms = true;
                     break;
                 }
             }
                 
         } while (hasEnemiesLvl0Rooms);
     }

     private void EnemiesRoomLevelTwo(DungeonData dungeonData)
     {
         Vector2Int enemiesLvl1RoomPosition = firstEnemiesRoomsPosition;
         
         do
         {
             hasEnemiesLvl1Rooms = false;
             
             foreach (var ennemiesRoom2Position in dungeonData.roomsDictionary.Keys)
             {
                 if (DijkstraAlgorithm.distanceDictionary[enemiesLvl1RoomPosition] > DijkstraAlgorithm.distanceDictionary[mapBoss] / 3)
                 {
                     enemiesLvl1RoomPosition = ennemiesRoom2Position;
                                               
                     var enemiesLvl1 = GetMapFromTilePosition(enemiesLvl1RoomPosition, dungeonData);
                     secondEnemiesRoomsPosition = enemiesLvl1;
                                               
                     spawnedObjects.AddRange(enemiesLvl1Room.ProcessRoom(secondEnemiesRoomsPosition, dungeonData.roomsDictionary[secondEnemiesRoomsPosition], dungeonData.GetRoomFloorWithoutCorridors(secondEnemiesRoomsPosition)));
                                               
                     dungeonData.roomsDictionary.Remove(secondEnemiesRoomsPosition);
                     hasEnemiesLvl1Rooms = true;
                     break;
                 }
             }
             
         } while (hasEnemiesLvl1Rooms);
     }
     
     private void EnemiesRoomLevelThree(DungeonData dungeonData)
     {
         Vector2Int enemiesLvl2RoomPosition = secondEnemiesRoomsPosition;
         
         do
         {
             hasEnemiesLvl2Rooms = false;
             
             foreach (var ennemiesRoom3Position in dungeonData.roomsDictionary.Keys)
             {
                 if (DijkstraAlgorithm.distanceDictionary[enemiesLvl2RoomPosition] > DijkstraAlgorithm.distanceDictionary[mapBoss] / 2)
                 {
                     enemiesLvl2RoomPosition = ennemiesRoom3Position;
                                               
                     var enemiesLvl2 = GetMapFromTilePosition(enemiesLvl2RoomPosition, dungeonData);
                     thirdEnemiesRoomsPosition = enemiesLvl2;
                                               
                     spawnedObjects.AddRange(enemiesLvl2Room.ProcessRoom(thirdEnemiesRoomsPosition, dungeonData.roomsDictionary[thirdEnemiesRoomsPosition], dungeonData.GetRoomFloorWithoutCorridors(thirdEnemiesRoomsPosition)));
                                               
                                               
                     dungeonData.roomsDictionary.Remove(thirdEnemiesRoomsPosition);
                     hasEnemiesLvl2Rooms = true;
                     break;
                 }
             }
            
         } while (hasEnemiesLvl2Rooms);
     }
     
     private void EnemiesRoomLevelFour(DungeonData dungeonData)
     {
         Vector2Int enemiesLvl3RoomPosition = thirdEnemiesRoomsPosition;
         
         do
         {
             hasEnemiesLvl3Rooms = false;

             foreach (var ennemiesRoom4Position in dungeonData.roomsDictionary.Keys)
             {
                 if (DijkstraAlgorithm.distanceDictionary[enemiesLvl3RoomPosition] > DijkstraAlgorithm.distanceDictionary[mapBoss])
                 {
                     enemiesLvl3RoomPosition = ennemiesRoom4Position;

                     var enemiesLvl3 = GetMapFromTilePosition(thirdEnemiesRoomsPosition, dungeonData);
                     lastEnemiesRoomsPosition = enemiesLvl3;

                     spawnedObjects.AddRange(enemiesLvl3Room.ProcessRoom(lastEnemiesRoomsPosition, dungeonData.roomsDictionary[lastEnemiesRoomsPosition], dungeonData.GetRoomFloorWithoutCorridors(lastEnemiesRoomsPosition)));
                     
                     dungeonData.roomsDictionary.Remove(lastEnemiesRoomsPosition);
                     hasEnemiesLvl3Rooms = true;
                     break;
                 }
             }

         } while (hasEnemiesLvl3Rooms);
     }
    
     #endregion*/

}
