using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AbstractDungeonGenerator générationDeMap;
    private void Start()
    {
        générationDeMap.GenerateDungeon();
    }
}
