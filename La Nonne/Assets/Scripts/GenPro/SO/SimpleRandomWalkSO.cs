using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters_", menuName = "PCG/SimpleRandomWalkData")]

public class SimpleRandomWalkSO : ScriptableObject
{
   [SerializeField] private int iterations = 10, walkLength = 10;
   [SerializeField] private bool startRandomlyEachIteration = true;
   
   public SimpleRandomWalk GetSimpleRandomWalk()
   {
      return new SimpleRandomWalk(iterations, walkLength, startRandomlyEachIteration);
   }
}

public class SimpleRandomWalk
{
   public int iterations, walkLength;
   public bool startRandomlyEachIteration;

   public SimpleRandomWalk(int i, int walkLength1, bool b)
   {
      iterations = i;
      walkLength = walkLength1;
      startRandomlyEachIteration = b;
   }
}
