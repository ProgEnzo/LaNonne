using UnityEngine;

public class BossGrowingCircleState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.GrowingCircleManager();
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
