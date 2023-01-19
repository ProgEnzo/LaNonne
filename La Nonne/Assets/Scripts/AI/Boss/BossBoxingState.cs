namespace AI.Boss
{
    public class BossBoxingState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.BoxingManager();
        }
    }
}
