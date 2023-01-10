using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.Boss;
using Cinemachine;
using Controller;
using GenPro.Rooms;
using Manager;
using Pathfinding;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Progress = Pathfinding.Progress;
using Random = UnityEngine.Random;

public class RoomContentGenerator : MonoBehaviour
{

    #region Variables
    
        [SerializeField, Space, Header ("RoomsTypes")]
        private RoomGenerator playerRoom, bossRoom, shopRoom, preBossRoom; //généraliser lvl0
        
        [SerializeField, Space, Header ("Tilemap")] 
        private TilemapVisualizer tilemapVisualizer;
        
        List<GameObject> spawnedObjects = new List<GameObject>();
        
        [SerializeField, Space, Header ("Dijkstra")]
        private GraphTest graphTest;

        [Header("Transforms")]
        public Transform itemParent;

        [SerializeField,Space, Header("Prefab Shop")]
        private GameObject prefabShop;
        
        [SerializeField,Space, Header("Prefab Boss")]
        private GameObject prefabBoss;
        
        [SerializeField,Space, Header("Prefab Boss")]
        private GameObject prefabHub;

        List<Vector2Int> shopRoomPos = new();
        
        [SerializeField,Space, Header("ShopApparitionFormula")] 
        private List<float> multiplierPerDistance = new(){20, 45, 54, 67, 100};
        
        [Header("CustomMap")]
        [SerializeField] private List<GameObject> wallUpCusto = new();
        [SerializeField] private List<GameObject> wallDownCusto = new();
        [SerializeField] private List<GameObject> wallRightCusto = new();
        [SerializeField] private List<GameObject> wallLeftCusto = new();
        [SerializeField] private List<GameObject> floorCusto = new();
        [SerializeField] private List<GameObject> floorNearWallsUpCusto = new();
        [SerializeField] private List<GameObject> floorNearWallsDownCusto = new();
        [SerializeField] private List<GameObject> floorNearWallsRightCusto = new();
        [SerializeField] private List<GameObject> floorNearWallsLeftCusto = new();

        #endregion
        
    private IEnumerator Scan()
    {
        yield return null;
        StartCoroutine(DoScan(0));
        yield return new WaitForSeconds(1);
        LoadingScreen.instance.HideLoadingScreen();
    }

    private IEnumerator DoScan(int size)
    {
        var graph = size == 1000 ? AstarPath.active.graphs[1]: AstarPath.active.graphs[0];
        foreach (Progress progress in AstarPath.active.ScanAsync(graph)) 
        {
            yield return null;
        }

        var graph2 = AstarPath.active.graphs[2];
        foreach (Progress progress in AstarPath.active.ScanAsync(graph2)) 
        {
            yield return null;
        }
        
        if (size < 1000)
        { 
            yield return null;
           StartCoroutine(DoScan(1000));
        }
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

        ModifyHubRoom();
        ModifyBossRoom();
        ModifyShopsRoom();

        #region teamWork avec le bro A* fonctionne bien

        int maxPosX = int.MinValue; //j'aime ca, J'aime Yalentin, ok
        int minPosX = int.MinValue;
        int maxPosY = int.MinValue;
        int minPosY = int.MinValue;

        var allRooms = dungeonData.roomsDictionary.Keys.ToList();
        allRooms.Add(bossRoom.roomCenter);

        foreach (var shops in shopRoomPos)
        {
            allRooms.Add(shops);
        }
        
        foreach (var roomData in allRooms)
        {
            maxPosX = maxPosX == int.MinValue ? roomData.x : Mathf.Max(maxPosX, roomData.x);
            minPosX = minPosX == int.MinValue ? roomData.x : Mathf.Min(minPosX, roomData.x);
            maxPosY = maxPosY == int.MinValue ? roomData.y: Mathf.Max(maxPosY, roomData.y);
            minPosY = minPosY == int.MinValue ? roomData.y : Mathf.Min(minPosY, roomData.y);
        }
        
        int gridPosX = (minPosX + maxPosX) / 2;
        int gridPosY = (minPosY + maxPosY) / 2;

        if (AstarPath.active.data.graphs[0] is GridGraph gPlayer)
        {
            gPlayer.center = new Vector3(playerSpawnRoomPosition.x, playerSpawnRoomPosition.y, 0);
        }
        
        if (AstarPath.active.data.graphs[2] is GridGraph gPlayer2)
        {
            gPlayer2.center = new Vector3(playerSpawnRoomPosition.x, playerSpawnRoomPosition.y, 0);
        }

        if (AstarPath.active.data.graphs[1] is GridGraph g)
        {
            g.center = new Vector3(gridPosX, gridPosY, 0);
            g.SetDimensions((int)((maxPosX - minPosX) / 0.45f) +40, (int)((maxPosY - minPosY) / 0.45f)+40, 0.45f);
        }
        #endregion

        foreach (var roomPos in copiedDico.Keys)  //pas besoin de ca si je le fais par salle
        {
            PopulateWallUp(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.WallUp)); //maybe les appeler que dans mes salle pour que ca n'apparaisse que sur les murs de certaines salles
            PopulateWallDown(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.WallDown));
            PopulateWallRight(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.wallRight));
            PopulateWallLeft(tilemapVisualizer.GetWalls(roomPos.x, roomPos.y, PlacementType.wallLeft));
            
            PopulateFloor(tilemapVisualizer.GetFloors(roomPos.x, roomPos.y)); //appeler ca dans la fonction qui fait aparaitre (playerRoom, etc....) les salles fait que l'on peux moduler pour chaque salle les objets qui apparaisse sur le sol

            PopulateFloorNearWallUp(tilemapVisualizer.GetFloorsNearWalls(roomPos.x, roomPos.y, PlacementType.nearWallUp));
            PopulateFloorNearWallRight(tilemapVisualizer.GetFloorsNearWalls(roomPos.x, roomPos.y, PlacementType.nearWallRight));
            PopulateFloorNearWallDown(tilemapVisualizer.GetFloorsNearWalls(roomPos.x, roomPos.y, PlacementType.nearWallDown));
            PopulateFloorNearWallLeft(tilemapVisualizer.GetFloorsNearWalls(roomPos.x, roomPos.y, PlacementType.nearWallLeft));
        }
        
        StartCoroutine(Scan());

        foreach (GameObject item in spawnedObjects)
        {
            if(item != null)
                item.transform.SetParent(itemParent, false);
        }
    }

    #region ModifyRooms
    private void ModifyBossRoom()
    {
        tilemapVisualizer.SwipeMap(bossRoom.roomCenter.x, bossRoom.roomCenter.y);
        InstantiateBossRoom(bossRoom.roomCenter.x, bossRoom.roomCenter.y);
        
    }
    
    private void ModifyHubRoom()
    {
        tilemapVisualizer.SwipeMap(playerRoom.roomCenter.x, playerRoom.roomCenter.y);
        InstantiateHubRoom(playerRoom.roomCenter.x, playerRoom.roomCenter.y);
    }
    
    private void ModifyShopsRoom()
    {
        foreach (var shops in shopRoomPos)
        {
            tilemapVisualizer.SwipeMap(shops.x, shops.y);
            InstantiateShop(shops.x, shops.y);
        }
    }
    
    #endregion
    
    #region InstantiateRooms
    private void InstantiateBossRoom(int x, int y)
    {
        var go = Instantiate(prefabBoss);
        Vector2Int pos = new Vector2Int(x, y);
        go.transform.position = new Vector3(x, y);
        var detector = go.GetComponent<DoorDetector>();
        detector.ManageDoors(hasTopMap(pos),hasBottomMap(pos), hasRightMap(pos),hasLeftMap(pos));
        
    }
    
    private void InstantiateHubRoom(int x, int y)
    {
        var go = Instantiate(prefabHub);
        Vector2Int pos = new Vector2Int(x, y);
        go.transform.position = new Vector3(x, y);
        var detector = go.GetComponent<DoorDetector>();
        detector.ManageDoors(hasTopMap(pos),hasBottomMap(pos), hasRightMap(pos),hasLeftMap(pos));
        
    }
    
    private void InstantiateShop(int x, int y)
    {
        var go = Instantiate(prefabShop);
        Vector2Int pos = new Vector2Int(x, y);
        go.transform.position = new Vector3(x, y);
        var detector = go.GetComponent<DoorDetector>();
        detector.ManageDoors(hasTopMap(pos),hasBottomMap(pos), hasRightMap(pos),hasLeftMap(pos));
    }
    #endregion
    
    #region HasMap
    private bool hasLeftMap(Vector2Int from)
    {
        return tilemapVisualizer.hasCorridor(from.x - 11, from.y);
    }
    
    private bool hasRightMap(Vector2Int from)
    {
        return tilemapVisualizer.hasCorridor(from.x + 11, from.y);
    }

    private bool hasTopMap(Vector2Int from)
    {
  
        return tilemapVisualizer.hasCorridor(from.x, from.y+11);
    }
    
    private bool hasBottomMap(Vector2Int from)
    {

        return tilemapVisualizer.hasCorridor(from.x, from.y-11);
    }
    #endregion

    #region PopulateMap
    public void PopulateWallUp (List<Vector2Int> wallUpPos)
    {
        foreach (var pos in wallUpPos)
        {
            if (Random.Range(0, 100) < 33)
            {
                GameObject item = Instantiate(wallUpCusto[Random.Range(0, wallUpCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    public void PopulateWallDown (List<Vector2Int> wallDownPos)
    {
        foreach (var pos in wallDownPos)
        {
            if (Random.Range(0, 100) < 0)
            {
                GameObject item = Instantiate(wallDownCusto[Random.Range(0, wallDownCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    public void PopulateWallRight (List<Vector2Int> wallRightPos)
    {
        foreach (var pos in wallRightPos)
        {
            if (Random.Range(0, 100) < 0)
            {
                GameObject item = Instantiate(wallRightCusto[Random.Range(0, wallRightCusto.Count)], new Vector3(pos.x, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    public void PopulateWallLeft (List<Vector2Int> wallLeftPos)
    {
        foreach (var pos in wallLeftPos)
        {
            if (Random.Range(0, 100) < 0)
            {
                GameObject item = Instantiate(wallLeftCusto[Random.Range(0, wallLeftCusto.Count)], new Vector3(pos.x +1f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    public void PopulateFloor (List<Vector2Int> floorPos)
    {
        foreach (var pos in floorPos)
        {
            if (Random.Range(0, 100) < 33)
            {
                GameObject item = Instantiate(floorCusto[Random.Range(0, floorCusto.Count)], new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }

    public void PopulateFloorNearWallUp(List<Vector2Int> floorNearWallsUpPos)
    {
        foreach (var pos in floorNearWallsUpPos)
        {
            if (Random.Range(0, 100) < 25)
            {
                GameObject item = Instantiate(floorNearWallsUpCusto[Random.Range(0, floorNearWallsUpCusto.Count)], new Vector3(pos.x + 0.5f, pos.y, 0), Quaternion.identity);
            }
        }
    }
    
    public void PopulateFloorNearWallDown(List<Vector2Int> floorNearWallsDownPos)
    {
        foreach (var pos in floorNearWallsDownPos)
        {
            if (Random.Range(0, 100) < 25)
            {
                GameObject item = Instantiate(floorNearWallsDownCusto[Random.Range(0, floorNearWallsDownCusto.Count)], new Vector3(pos.x, pos.y + 1.5f, 0), Quaternion.identity);
            }
        }
    }
    
    public void PopulateFloorNearWallRight(List<Vector2Int> floorNearWallsRightPos)
    {
        foreach (var pos in floorNearWallsRightPos)
        {
            if (Random.Range(0, 100) < 25)
            {
                GameObject item = Instantiate(floorNearWallsRightCusto[Random.Range(0, floorNearWallsRightCusto.Count)], new Vector3(pos.x - 1.5f, pos.y + 0.5f, 0), Quaternion.identity);
            }
        }
    }
    
    public void PopulateFloorNearWallLeft(List<Vector2Int> floorNearWallsLeftPos)
    {
        foreach (var pos in floorNearWallsLeftPos)
        {
            if (Random.Range(0, 100) < 25)
            {
                GameObject item = Instantiate(floorNearWallsLeftCusto[Random.Range(0, floorNearWallsLeftCusto.Count)], new Vector3(pos.x + 1.4f, pos.y + 0.3f, 0), Quaternion.identity);
            }
        }
    }
    
    #endregion

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

    public Vector2Int firstShopPosition;
    public Vector2Int lastShopPosition;
    private void SelectShopSpawnPoints(DungeonData dungeonData)
    {
        Vector2Int shopRoomPosition = playerSpawnRoomPosition;
        foreach (var shop in dungeonData.roomsDictionary.Keys) 
        {
            if (DijkstraAlgorithm.distanceDictionary [shop] == 44)
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
                else if (distance <= 2) 
                    spawnedObjects.AddRange(enemyRoom[1].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance <= 4)  //Pyro Tutorial
                    spawnedObjects.AddRange(enemyRoom[2].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance <= 5) 
                    spawnedObjects.AddRange(enemyRoom[3].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance <= 6) 
                    spawnedObjects.AddRange(enemyRoom[4].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance <= 7) //TDI Tutorial
                    spawnedObjects.AddRange(enemyRoom[5].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                else if (distance > 7) 
                    spawnedObjects.AddRange(enemyRoom[6].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
                // else if (distance > 9) 
                //     spawnedObjects.AddRange(enemyRoom[7].ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
            }
        }
        
        #endregion
        
    #region PreBossRoom
        
        private void PreBossRoomPosition(DungeonData dungeonData)
        {
            Vector2Int preBossRoomPosition = playerSpawnRoomPosition;
            
            foreach (var preBoss in dungeonData.roomsDictionary.Keys)
            {
                if (DijkstraAlgorithm.distanceDictionary[preBoss] == DijkstraAlgorithm.distanceDictionary[mapBoss] - 44)
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
}
