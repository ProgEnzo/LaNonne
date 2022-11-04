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

    
    [Header("Dash")]
    
    public int dashDamage;
    public int bodyDamage;
    
    public float dashDistance;
    public float dashTime;
    public float dashCooldown;
    
    public int dashAmount = 3;

    [Header("AttackCircle")] 
    public GameObject circle;
    public int circleDamage;
    public int circleAmount = 3;
    public float circleSpacingCooldown;
    
    
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
            player.TakeDamage(dashDamage);
        }

        if (circle.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(circleDamage);
            Debug.Log($"<color=green>DASHING STATE HAS BEGUN</color>");
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(bodyDamage);
        }
    }

    public void SwitchState(BossBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    #region DashingState
    public void DashManager()
    {
        StartCoroutine(Dash());
        Debug.Log($"<color=red>DASHING STATE HAS BEGUN</color>");

    }

    private IEnumerator Dash()
    {
        dashAmount--; //décrémente de 1 le nombre de dash restant
        yield return new WaitForSeconds(1);

        
        
        player.GetComponent<Collider2D>().isTrigger = true;
        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        Vector2 direction = player.transform.position - transform.position;
        rb.velocity = direction.normalized * dashDistance;
        yield return new WaitForSeconds(dashTime);

        
        player.GetComponent<Collider2D>().isTrigger = false;
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        yield return new WaitForSeconds(dashCooldown);
        
       
        if (dashAmount > 0)
        {
            StartCoroutine(Dash());
        }
        else
        {
            SwitchState(AttackCircleState); //SWITCH INTO BossAttackCircleState
        }

    }
    

    #endregion

    #region AttackCircleState

    public void AttackCircleManager()
    {
        StartCoroutine(AttackCircle());
        Debug.Log($"<color=red>ATTACK CIRCLE STATE HAS BEGUN</color>");
    }
    
    private IEnumerator AttackCircle()
    {
        circleAmount--;
        GameObject circleObject;
        
        yield return new WaitForSeconds(circleSpacingCooldown);

        circleObject = Instantiate(circle, player.transform.position, Quaternion.identity);
        
        yield return new WaitForSeconds(circleSpacingCooldown);
        Destroy(circleObject, 1f);

        if (circleAmount > 0)
        {
            StartCoroutine(AttackCircle());
        }
        else
        {
            SwitchState(GrowingCircleState); //SWITCH INTO GrowingCircleState
        }
    }

    #endregion
    
}
