using UnityEngine;

public class BossDashingState : BossBaseState
{
    
    public override void EnterState(BossStateManager boss)
    {
        boss.StartCoroutine(boss.Dashing()); //Comment je fais pour que à la fin de cette coroutine, j'applique le switch

        
        //boss.SwitchState(boss.AttackCircleState);
        Debug.Log("Hello from the DashingState :))");
        
    }

    public override void UpdateState(BossStateManager boss)
    {
        
    }
}
