namespace AI.Boss
{
    public class BossVacuumState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.VacuumManager();
        }
    }
}
