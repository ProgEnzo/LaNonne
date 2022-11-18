using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Controller;
using DG.Tweening;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossStateManager : MonoBehaviour
{
    private BossBaseState currentState;
    public BossDashingState DashingState = new BossDashingState();
    public BossThrowingState ThrowingState = new BossThrowingState();
    public BossAttackCircleState AttackCircleState = new BossAttackCircleState();
    public BossVacuumState VacuumState = new BossVacuumState();
    public BossMineState MineState = new BossMineState();
    public BossTransitionState TransitionState = new BossTransitionState();

    public Rigidbody2D rb;
    public PlayerController player;
    public AIPath bossAI;
    
    //LIST
    public List<BossBaseState> firstStatesList = new List<BossBaseState>();
    public List<GameObject> spawnerList = new List<GameObject>();
    
    [Header("Overall Stats")]
    public Slider hpBossSlider;
    public int currentHealth;
    public int maxHealth;
    public float normalSpeed;

    [Header("----Dash----")] 
    public GameObject dashMine;
    public int bodyDamage;
    
    public float dashDistance;
    public float dashTime;
    public float dashCooldown;
    public float timeBeforeDashing;
    
    public int dashAmount;
    public int currentDashAmount;

    [Header("----AttackCircle----")]
    public GameObject attackCircleWarning;
    public GameObject attackCircle;
    public float attackCircleSpacingCooldown;
    public int attackCircleAmount;
    public int currentAttackCircleAmount;

    [Header("----Vacuum----")]
    public GameObject vacuumArea;
    public float vacuumCooldown;
    public float toxicAreaCooldown;
    public int vacuumAmount;
    public int currentVacuumAmount;

    [Header("----TransitionState----")] 
    public CinemachineVirtualCamera vCamPlayer;
    public bool takingDamage = true;
    public int numberOfSpawn;

    void Start()
    {
        currentState = DashingState; //starting state for the boss state machine
        currentState.EnterState(this); //"this" is this Monobehavior script
        
        //HEALTH
        currentHealth = maxHealth;
        hpBossSlider.maxValue = maxHealth;
        hpBossSlider.value = maxHealth;

        bossAI.maxSpeed = normalSpeed;
        
        //STATES
        firstStatesList.Add(DashingState);
        firstStatesList.Add(AttackCircleState);
        firstStatesList.Add(VacuumState);

        //VIRTUAL CAMERA
        vCamPlayer = GameObject.Find("vCamPlayer").GetComponent<CinemachineVirtualCamera>();
    }
    void Update()
    {
        currentState.UpdateState(this); //will call any code in Update State from the current state every frame
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            
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
        Reinit();
        currentState = state;
        state.EnterState(this);
        
    }

    void Reinit()
    {
        currentDashAmount = dashAmount;
        currentAttackCircleAmount = attackCircleAmount;
        currentVacuumAmount = vacuumAmount;
    }
    
    #region Health Boss

    public void TakeDamageOnBossFromPlayer(int damage)
    {
        if (takingDamage)
        {
            currentHealth -= damage;
            hpBossSlider.value -= damage;
        }
        

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
        Debug.Log($"<color=green>DASHING STATE HAS BEGUN</color>");

    }

    

    private IEnumerator Dash()
    {
        
        currentDashAmount--; //décrémente de 1 le nombre de dash restant
        yield return new WaitForSeconds(1);

        bossAI.maxSpeed = 0;
        yield return new WaitForSeconds(timeBeforeDashing);

        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        Vector2 direction = player.transform.position - transform.position;
        rb.velocity = direction.normalized * dashDistance; // DASH
        Physics2D.IgnoreLayerCollision(15, 7, true);

        StartCoroutine(DashMine());
        yield return new WaitForSeconds(0.3f);
        
        StartCoroutine(DashMine());
        yield return new WaitForSeconds(0.3f);
        
        StartCoroutine(DashMine());
        yield return new WaitForSeconds(dashTime);
        
        bossAI.maxSpeed = normalSpeed;
        Physics2D.IgnoreLayerCollision(15, 7, false);

        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        yield return new WaitForSeconds(dashCooldown);
        
       
        if (currentDashAmount > 0 && currentHealth >= maxHealth / 2)
        {
            StartCoroutine(Dash());
        }
        else if (currentDashAmount == 0)
        {
            var nextState = firstStatesList[Random.Range(0, firstStatesList.Count)];
            
            SwitchState(nextState);
        }
        else if (currentHealth <= maxHealth / 2)
        {
            SwitchState(TransitionState);
        }
    }

    private IEnumerator DashMine()
    {
        var dashMineObject = Instantiate(dashMine, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        
        dashMineObject.transform.DOScale(new Vector3(3, 3, 0), 3f);
        Destroy(dashMineObject, 3f);
        yield return new WaitForSeconds(0.3f);
    }
    

    #endregion

    #region AttackCircleState

    public void AttackCircleManager()
    {
        StartCoroutine(AttackCircle());
        Debug.Log($"<color=green>ATTACK CIRCLE STATE HAS BEGUN</color>");
    }
    
    private IEnumerator AttackCircle()
    {
        bossAI.maxSpeed = 0;
        currentAttackCircleAmount--;
        yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
        var circleObjectWarning = Instantiate(attackCircleWarning, player.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
        Destroy(circleObjectWarning, 1f);
        var circleObject = Instantiate(attackCircle, circleObjectWarning.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
        Destroy(circleObject, 1f);

        if (currentAttackCircleAmount > 0 && currentHealth >= maxHealth / 2)
        {
            StartCoroutine(AttackCircle());
        }
        else if (currentAttackCircleAmount == 0)
        {
            var nextState = firstStatesList[Random.Range(0, firstStatesList.Count)];
            
            SwitchState(nextState);
        }
        else if (currentHealth <= maxHealth / 2)
        {
            bossAI.maxSpeed = normalSpeed;
            SwitchState(TransitionState);
        }
    }

    #endregion
    
    #region VacuumState

    public void VacuumManager()
    {
        StartCoroutine(Vacuum());
        Debug.Log($"<color=green>SHRINKING CIRCLE STATE HAS BEGUN</color>");

    }
    
    private IEnumerator Vacuum()
    {
        currentVacuumAmount--;
        bossAI.maxSpeed = 1f; // decrease speed
        yield return new WaitForSeconds(2f);

        bossAI.maxSpeed = 0f; // STOP THE BOSS
        
        var shrinkingCircleGameObject = Instantiate(vacuumArea, transform.position, Quaternion.identity);
        shrinkingCircleGameObject.transform.parent = gameObject.transform; //set le prefab en child
        StartCoroutine(ToxicArea());
        yield return new WaitForSeconds(vacuumCooldown);

        Destroy(shrinkingCircleGameObject);
        bossAI.maxSpeed = normalSpeed; //Change the speed back to normal
        

        if (currentVacuumAmount > 0 && currentHealth >= maxHealth / 2)
        {
            StartCoroutine(Vacuum());
        }
        else if(currentVacuumAmount == 0)
        {
            var nextState = firstStatesList[Random.Range(0, firstStatesList.Count)];
            
            SwitchState(nextState);
        }
        else if (currentHealth <= maxHealth / 2)
        {
            bossAI.maxSpeed = normalSpeed;
            SwitchState(TransitionState);
        }
        
    }

    private IEnumerator ToxicArea()
    {
        
        yield return new WaitForSeconds(1f);
    }
    #endregion

    #region TransitionState

    public void TransitionManager()
    {
        StartCoroutine(Transition());
        Debug.Log($"<color=orange>TRANSITION STATE HAS BEGUN</color>");
    }

    private IEnumerator Transition()
    {
        bossAI.maxSpeed = 0;
        vCamPlayer.Priority = 1; //on laisse la priorité à la vCam du boss
        takingDamage = false;
        yield return new WaitForSeconds(3f);
        
        vCamPlayer.Priority = 10;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < numberOfSpawn; i++)
        {
            var spawnEnemy = Instantiate(spawnerList[Random.Range(0, spawnerList.Count)], new Vector2(Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity);
        }

        
        yield return new WaitForSeconds(10f);
        bossAI.maxSpeed = normalSpeed;
        takingDamage = true;
        SwitchState(MineState);

    }

    #endregion

    #region MineState

    public void MineManager()
    {
        StartCoroutine(Mine());
        Debug.Log($"<color=red>MINE STATE HAS BEGUN</color>");

    }

    private IEnumerator Mine()
    {
        yield return new WaitForSeconds(1f);
    }

    #endregion
}
