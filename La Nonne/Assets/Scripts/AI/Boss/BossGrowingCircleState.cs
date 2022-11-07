using UnityEngine;

public class BossGrowingCircleState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.GrowingCircleManager();
        Debug.Log("Hello from the GROWING CIRCLE STATE :))");

    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
