using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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
    public AIPath bossAI;

    
    [Header("Health")]
    public int currentHealth;
    public Slider hpBossSlider;
    
    [Header("----Dash----")]
    public int dashDamage;
    public int bodyDamage;
    
    public float dashDistance;
    public float dashTime;
    public float dashCooldown;
    
    public int dashAmount = 3;

    [Header("----AttackCircle----")]
    public GameObject attackCircleWarning;
    public GameObject attackCircle;
    public float attackCircleSpacingCooldown;
    public int attackCircleAmount = 3;

    [Header("----GrowingCircle----")] 
    public GameObject growingCircle;
    public float growingCircleCooldown;
    public int growingCircleAmount;

    [Header("----ShrinkingCircle----")]
    public GameObject shrinkingCircle;
    public float shrinkingCircleCooldown;
    public int shrinkingCircleAmount;

    public GameObject rotatingBlade;
    public float rotatingBladeCooldown;
    
    void Start()
    {
        currentState = ShrinkingCircleState; //starting state for the boss state machine
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

    #region Health Boss

    public void TakeDamageFromPlayer(int damage)
    {
        currentHealth -= damage;
        hpBossSlider.value -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    #endregion
    
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
        attackCircleAmount--;
        yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
        var circleObjectWarning = Instantiate(attackCircleWarning, player.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
        Destroy(circleObjectWarning, 1f);
        var circleObject = Instantiate(attackCircle, circleObjectWarning.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
        Destroy(circleObject, 1f);

        if (attackCircleAmount > 0)
        {
            StartCoroutine(AttackCircle());
        }
        else
        {
            SwitchState(GrowingCircleState); //SWITCH INTO GrowingCircleState
        }
    }

    #endregion

    #region GrowingCircleState
    public void GrowingCircleManager()
    {
        StartCoroutine(GrowingCircle());
        Debug.Log($"<color=red>GROWING CIRCLE STATE HAS BEGUN</color>");
    }
    
    private IEnumerator GrowingCircle()
    {
        growingCircleAmount--;
        
        yield return new WaitForSeconds(growingCircleCooldown);
        
        var growingCircleGameObject = Instantiate(growingCircle, transform.position, quaternion.identity);
        growingCircleGameObject.transform.DOKill();
        growingCircleGameObject.transform.localScale = Vector3.zero;
        growingCircleGameObject.transform.DOScale(new Vector3(10, 10, 0), 3f);
        yield return new WaitForSeconds(growingCircleCooldown);
        
        Destroy(growingCircleGameObject, 3f);

        if (growingCircleAmount > 0)
        {
            StartCoroutine(GrowingCircle());
        }
        else
        {
            SwitchState(ShrinkingCircleState);
        }
    }
    #endregion

    #region ShrinkingCircleState

    public void ShrinkingCircleManager()
    {
        StartCoroutine(ShrinkingCircle());
        Debug.Log($"<color=red>SHRINKING CIRCLE STATE HAS BEGUN</color>");

    }
    
    private IEnumerator ShrinkingCircle()
    {
        shrinkingCircleAmount--;
        yield return new WaitForSeconds(shrinkingCircleCooldown);
        
        RotatingBlade();
        yield return new WaitForSeconds(rotatingBladeCooldown);
        
        //METTRE CE BLOC DANS LA COROUTINE
        var shrinkingCircleGameObject = Instantiate(shrinkingCircle, transform.position, quaternion.identity);
        shrinkingCircleGameObject.transform.parent = gameObject.transform; //set le prefab en child
        shrinkingCircleGameObject.transform.DOKill();
        shrinkingCircleGameObject.transform.DOScale(new Vector3(0, 0, 0), 3f);
        yield return new WaitForSeconds(shrinkingCircleCooldown);

        Destroy(shrinkingCircleGameObject, 3f);
        
        if (shrinkingCircleAmount > 0)
        {
            StartCoroutine(ShrinkingCircle());
        }
        else
        {
            SwitchState(TransitionState);
        }
    }

    private void RotatingBlade()
    {
        bossAI.maxSpeed = 1;
        var rotatingBladeGameObject = Instantiate(rotatingBlade, transform.position, Quaternion.identity);
        rotatingBladeGameObject.transform.parent = gameObject.transform;
        rotatingBladeGameObject.transform.DORotate(new Vector3(0, 0, 360), rotatingBladeCooldown, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
        
    }
    

    #endregion
}
