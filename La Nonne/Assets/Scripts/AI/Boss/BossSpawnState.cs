namespace AI.Boss
{
    public class BossSpawnState : BossBaseState
    {
        public override void EnterState(BossStateManager boss)
        {
            boss.SpawnEnemyManager();
        }
    }
}
