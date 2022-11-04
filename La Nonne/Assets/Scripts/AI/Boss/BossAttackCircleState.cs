using UnityEngine;

public class BossAttackCircleState : BossBaseState
{
    public override void EnterState(BossStateManager boss)
    {
        Debug.Log("Hello from the ATTACK CIRCLE STATE");
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
