namespace AI.Boss
{
    public class BossStartingState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.StartManager();
        }
    }
}
