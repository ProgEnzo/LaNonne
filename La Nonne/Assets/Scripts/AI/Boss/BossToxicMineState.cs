using AI.Boss;
using UnityEngine;

public class BossToxicMineState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.ToxicMineManager();

    }

    public override void UpdateState(BossStateManager boss)
    {

    }
}
