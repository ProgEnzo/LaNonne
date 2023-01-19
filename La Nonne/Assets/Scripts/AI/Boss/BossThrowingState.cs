namespace AI.Boss
{
    public class BossThrowingState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.ThrowingManager();
        }
    }
}
