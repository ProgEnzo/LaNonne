namespace AI.Boss
{
    public class BossDashingState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.DashManager();
        }
    }
}
