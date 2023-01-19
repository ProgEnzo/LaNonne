namespace AI.Boss
{
    public class BossTransitionState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.TransitionManager();
        }
    }
}
