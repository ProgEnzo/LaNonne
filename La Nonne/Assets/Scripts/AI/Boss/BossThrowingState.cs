using UnityEngine;

public class BossThrowingState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.ThrowingManager();
    }

    public override void UpdateState(BossStateManager boss)
    {
        boss.SwitchFromThrowingToBoxing();
    }
}
