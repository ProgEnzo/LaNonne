using System.Collections;
using System.Collections.Generic;
using Controller;
using Pathfinding;
using UnityEngine;

public class BossStateManager : MonoBehaviour
{
    private BossBaseState currentState;
    public BossDashingState DashingState = new BossDashingState();
    public BossShootingState ShootingState = new BossShootingState();
    public BossAttackCircleState AttackCircleState = new BossAttackCircleState();
    public BossGrowingCircleState GrowingCircleState = new BossGrowingCircleState();
    public BossShrinkingCircleState ShrinkingCircleState = new BossShrinkingCircleState();
    public BossRotateAroundState RotateAroundState = new BossRotateAroundState();
    public BossTransitionState TransitionState = new BossTransitionState();

    public Rigidbody2D rb;
    public PlayerController player;
    public float dashPower;
    void Start()
    {
        currentState = DashingState; //starting state for the boss state machine
        currentState.EnterState(this); //"this" is this Monobehavior script
    }
    void Update()
    {
        currentState.UpdateState(this); //will call any code in Update State from the current state every frame
        
    }

    public void SwitchState(BossBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public IEnumerator Dashing()
    {
        yield return new WaitForSeconds(1f);

        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        Vector2 direction = player.transform.position - transform.position;
        rb.AddForce(direction.normalized * dashPower, ForceMode2D.Impulse);
        Debug.Log(direction);

        yield return new WaitForSeconds(1f);
        
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        yield return new WaitForSeconds(1f);
        
        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        Vector2 direction2 = player.transform.position - transform.position;
        rb.AddForce(direction2.normalized * dashPower, ForceMode2D.Impulse);
        Debug.Log(direction2);

        yield return new WaitForSeconds(1f);
        
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        yield return new WaitForSeconds(1f);
        
        
    }
}
