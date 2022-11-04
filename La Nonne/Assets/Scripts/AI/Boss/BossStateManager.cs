using System;
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
    public float dashTime;
    public float dashCooldown;
    public bool canDash = true;
    public bool isDashing;
    public int dashAmount = 3;
    
    void Start()
    {
        currentState = DashingState; //starting state for the boss state machine
        currentState.EnterState(this); //"this" is this Monobehavior script
    }
    void Update()
    {
        currentState.UpdateState(this); //will call any code in Update State from the current state every frame
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(20);
        }
    }

    public void SwitchState(BossBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void DashManager()
    {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        dashAmount--;
        Debug.Log(dashAmount);
        yield return new WaitForSeconds(1);

        player.GetComponent<Collider2D>().isTrigger = true;
        canDash = false;
        isDashing = true;
        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        Vector2 direction = player.transform.position - transform.position;
        rb.velocity = direction.normalized * dashPower;
        yield return new WaitForSeconds(dashTime);

        isDashing = false;
        player.GetComponent<Collider2D>().isTrigger = false;
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        yield return new WaitForSeconds(dashCooldown);
        
        canDash = true;
        if (dashAmount > 0)
        {
            StartCoroutine(Dash());
        }
        else
        {
            SwitchState(AttackCircleState); //SWITCH INTO BossAttackCircleState
        }

    }
}
