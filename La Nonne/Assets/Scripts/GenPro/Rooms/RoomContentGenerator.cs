using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class RoomContentGenerator : MonoBehaviour
{
    [SerializeField]
    private RoomGenerator playerRoom, defaultRoom, bossRoom, shopRoom;

    List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField]
    private GraphTest graphTest;


    public Transform itemParent;

    [SerializeField]
    private CinemachineVirtualCamera cinemachineCamera;
    public void GenerateRoomContent(DungeonData dungeonData)
    {
        foreach (GameObject item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        SelectPlayerSpawnPoint(dungeonData);
        SelectEnemySpawnPoints(dungeonData);
        SelectBossSpawnPoints(dungeonData);
        SelectShopSpawnPoints(dungeonData);

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
    
    private void SelectPlayerSpawnPoint(DungeonData dungeonData)
    {
        int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.roomsDictionary.Count);
        Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

        graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

        Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

        List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
            playerSpawnPoint,
            dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
            );

        FocusCameraOnThePlayer(placedPrefabs[placedPrefabs.Count - 1].transform);

        spawnedObjects.AddRange(placedPrefabs);

        dungeonData.roomsDictionary.Remove(playerSpawnPoint);
    }
   
    private void SelectEnemySpawnPoints(DungeonData dungeonData)
    {
        foreach (KeyValuePair<Vector2Int,HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary)
        { 
            spawnedObjects.AddRange(defaultRoom.ProcessRoom(roomData.Key, roomData.Value, dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)));
        }
    }

    private void SelectBossSpawnPoints(DungeonData dungeonData)
    {
       //DijkstraAlgorithm.Dijkstra(Graph, dungeonData.roomsDictionary.Keys.ElementAt(0));
    }
    
    private void SelectShopSpawnPoints(DungeonData dungeonData)
    {
        /*int randomRoomIndex = UnityEngine.Random.Range(5, dungeonData.roomsDictionary.Count);
        Vector2Int bossSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);
        
        graphTest.RunDijkstraAlgorithm(bossSpawnPoint, dungeonData.floorPositions);
        
        Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);
        
        List<GameObject> placedPrefabs = bossRoom.ProcessRoom(
            bossSpawnPoint,
            dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
        );

        spawnedObjects.AddRange(placedPrefabs);

        dungeonData.roomsDictionary.Remove(bossSpawnPoint);*/
    }
}
