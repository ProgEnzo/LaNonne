using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public Sprite sprite;
    public Vector2 size = new Vector2(0.5f, 0.5f);
    
    public PlacementType placementType;
    
    public bool addOffset;

    //public GameObject prefab;

    //public int health = 1;
    //public bool nonDestructible;
}
