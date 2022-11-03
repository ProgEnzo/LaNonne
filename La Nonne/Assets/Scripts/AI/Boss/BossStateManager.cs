using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateManager : MonoBehaviour
{
    private BossBaseState currentState;
    private BossDashingState DashingState = new BossDashingState();
    private BossShootingState ShootingState = new BossShootingState();
    private BossAttackCircleState AttackCircleState = new BossAttackCircleState();
    private BossGrowingCircleState GrowingCircleState = new BossGrowingCircleState();
    private BossShrinkingCircleState ShrinkingCircleState = new BossShrinkingCircleState();
    private BossRotateAroundState RotateAroundState = new BossRotateAroundState();
    private BossTransitionState TransitionState = new BossTransitionState();
    void Start()
    {
        //starting state for the boss state machine
        currentState = DashingState;
        
        //"this" is this Monobehavior script
        currentState.EnterState(this);
    }
    void Update()
    {
        
    }
}
