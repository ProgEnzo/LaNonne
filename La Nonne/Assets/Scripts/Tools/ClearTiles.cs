using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tools
{
    [ExecuteInEditMode]
    public class ClearTiles : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Tilemap>().ClearAllTiles();
            DestroyImmediate(this);
        }
    }
}