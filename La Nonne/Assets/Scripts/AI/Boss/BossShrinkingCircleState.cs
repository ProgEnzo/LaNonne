using UnityEngine;

public class BossShrinkingCircleState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.ShrinkingCircleManager();

    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
