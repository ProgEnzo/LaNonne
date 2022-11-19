using UnityEngine;

public class BossVacuumState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.VacuumManager();

    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
