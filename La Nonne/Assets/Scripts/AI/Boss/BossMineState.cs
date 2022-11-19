using UnityEngine;

public class BossMineState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.MineManager();
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
