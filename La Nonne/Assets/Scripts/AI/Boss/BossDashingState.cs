using UnityEngine;

public class BossDashingState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.DashManager(); 
        
        //boss.SwitchState(boss.AttackCircleState);

        
        Debug.Log("Hello from the DashingState :))");
        
    }

    public override void UpdateState(BossStateManager boss)
    {

    }
}
