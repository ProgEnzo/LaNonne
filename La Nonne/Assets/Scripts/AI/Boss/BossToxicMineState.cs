namespace AI.Boss
{
    public class BossToxicMineState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.ToxicMineManager();
        }
    }
}
