using UnityEngine;

public class BossTransitionState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        Debug.Log("Hello from the TRANSITION STATE");
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
