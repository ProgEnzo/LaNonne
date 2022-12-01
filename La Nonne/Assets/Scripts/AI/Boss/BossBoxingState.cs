using System.Collections;
using System.Collections.Generic;
using AI.Boss;
using UnityEngine;

public class BossBoxingState : BossBaseState
{
    public override void EnterState(BossStateManager boss)
    {
        boss.BoxingManager();

    }

    public override void UpdateState(BossStateManager boss)
    {


    }
}
