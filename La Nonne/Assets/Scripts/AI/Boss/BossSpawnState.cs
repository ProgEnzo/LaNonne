using AI.Boss;
using UnityEngine;

public class BossSpawnState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.SpawnEnemyManager();

    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
