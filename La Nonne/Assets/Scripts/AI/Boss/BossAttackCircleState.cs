namespace AI.Boss
{
    public class BossAttackCircleState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.AttackCircleManager();
        }
    }
}
