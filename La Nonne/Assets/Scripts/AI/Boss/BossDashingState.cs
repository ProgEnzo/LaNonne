using AI.Boss;
using UnityEngine;

public class BossDashingState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.DashManager();
        
    }

    public override void UpdateState(BossStateManager boss)
    {

    }
}
