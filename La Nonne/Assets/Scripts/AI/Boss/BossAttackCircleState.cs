using AI.Boss;
using UnityEngine;

public class BossAttackCircleState : BossBaseState
{
    public override void EnterState(BossStateManager boss)
    {
        boss.AttackCircleManager();
        
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
