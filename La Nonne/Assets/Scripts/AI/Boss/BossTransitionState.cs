using UnityEngine;

public class BossTransitionState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.TransitionManager();
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
