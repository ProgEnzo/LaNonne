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
    public BossToxicMineState ToxicMineState = new BossToxicMineState();
    public BossTransitionState TransitionState = new BossTransitionState();

    public Rigidbody2D rb;
    private PlayerController player;
    public AIPath bossAI;
    
    //LIST
    public List<BossBaseState> firstStatesList = new List<BossBaseState>();
    public List<BossBaseState> lastStatesList = new List<BossBaseState>();
    
    public List<GameObject> spawnerList = new List<GameObject>();

    [Header("Overall Stats")]
    [SerializeField] private Slider hpBossSlider;
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
    public GameObject toxicArea;
    public GameObject vacuumParticle;
    public float vacuumCooldown;
    public float toxicAreaCooldown;
    public int vacuumAmount;
    public int currentVacuumAmount;

    [Header("----TransitionState----")] 
    public CinemachineVirtualCamera vCamPlayer;
    public bool takingDamage = true;
    public int numberOfSpawn;
    public float transitionCooldown;

    [Header("----ToxicMineState----")]
    public List<GameObject> toxicMineList = new List<GameObject>();
    public List<GameObject> toxicMineAreaWarningList = new List<GameObject>();
    public List<GameObject> toxicMineAreaList = new List<GameObject>();

    private GameObject toxicMineObject;
    public GameObject toxicMine;
    public GameObject toxicMineWarning;
    public GameObject toxicMineArea;
    
    
    public int numberOfToxicMines;
    
    public int toxicMineAmount;
    public int currentToxicMineAmount;
    
    [Header("----ThrowingState----")] 
    public GameObject slug;
    public int throwAmount;
    public int currentThrowAmount;

    private void Awake()
    {
      
    }

    void Start()
    {
        hpBossSlider = GameObject.FindGameObjectWithTag("Boss HealthBar").GetComponent<Slider>();
        player = PlayerController.instance;
        gameObject.GetComponent<AIDestinationSetter>().target = PlayerController.instance.transform;

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
        
        lastStatesList.Add(ThrowingState);

        //VIRTUAL CAMERA
        vCamPlayer = GameObject.Find("vCamPlayer").GetComponent<CinemachineVirtualCamera>();
    }
    void Update()
    {
        currentState.UpdateState(this); //will call any code in Update State from the current state every frame
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

        currentToxicMineAmount = toxicMineAmount;
        currentThrowAmount = throwAmount;
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
        
        dashMineObject.transform.DOScale(new Vector2(3, 3), 3f);
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
        
        var bossPos = transform.position;

        //VACUUM AREA
        var vacuumGameObject = Instantiate(vacuumArea, bossPos, Quaternion.identity);
        vacuumGameObject.transform.parent = gameObject.transform; //set le prefab en child
        vacuumParticle.SetActive(true);
        
        //TOXIC AREA
        var toxicAreaObject = Instantiate(toxicArea, bossPos, Quaternion.identity);
        toxicAreaObject.transform.DOScale(new Vector2(2.5f, 2.5f), 2f);
        yield return new WaitForSeconds(vacuumCooldown);

        Destroy(vacuumGameObject);
        vacuumParticle.SetActive(false);
        Destroy(toxicAreaObject);
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
        
        for (int i = 0; i < numberOfSpawn; i++)
        {
            yield return new WaitForSeconds(0.3f);
            
            var posBossBeforeSpawn = transform.position; //get la pos du boss
            var spawnEnemy = Instantiate(spawnerList[Random.Range(0, spawnerList.Count)], new Vector2(posBossBeforeSpawn.x + Random.Range(-2f, 2f),posBossBeforeSpawn.y +  Random.Range(-2f, 2f)), Quaternion.identity);
        }
        yield return new WaitForSeconds(1f);

        vCamPlayer.Priority = 10;
        yield return new WaitForSeconds(transitionCooldown);
        
        //CODE FOR THE EXPLOSION AFTER THE TRANSITION HERE
        
        bossAI.maxSpeed = normalSpeed;
        takingDamage = true;
        SwitchState(ThrowingState);

    }

    #endregion

    #region ToxicMineState

    public void ToxicMineManager()
    {
        StartCoroutine(ToxicMine());
        Debug.Log($"<color=red>TOXIC MINE STATE HAS BEGUN</color>");

    }

    private IEnumerator ToxicMine()
    {
        currentToxicMineAmount--;
        yield return new WaitForSeconds(1f);

        //SPAWN TOXIC MINE 
        for (int i = 0; i < numberOfToxicMines; i++)
        {
            var posBossBeforeSpawn = transform.position; //get la pos du boss 
            
            toxicMineObject = Instantiate(toxicMine, posBossBeforeSpawn, Quaternion.identity); //spawn toxic mines
            toxicMineObject.transform.DOMove(new Vector2(posBossBeforeSpawn.x + Random.Range(-5f, 5f),posBossBeforeSpawn.y +  Random.Range(-5f, 5f)), 1.5f).SetEase(Ease.OutQuint); //send toxic mines everywhere
            
            toxicMineList.Add(toxicMineObject); //add toxic mines into the list
        }
        
        yield return new WaitForSeconds(1.5f);

        //SPAWN TOXIC MINE AREA WARNING
        for (int i = 0; i < numberOfToxicMines; i++)
        {
            var toxicMineAreaWarningObject = Instantiate(toxicMineWarning, toxicMineList[i].transform.position, Quaternion.identity);
            
            //ADD LIST Mine Area Warning
            toxicMineAreaWarningList.Add(toxicMineAreaWarningObject);
        }
        
        foreach (var VARIABLE in toxicMineAreaWarningList)
        {
            Destroy(VARIABLE, 1f); //destroy toxic mine
        }
        
        yield return new WaitForSeconds(1f);

        //SPAWN TOXIC MINE AREA
        for (int i = 0; i < numberOfToxicMines; i++)
        {
            var toxicMineAreaObject = Instantiate(toxicMineArea, toxicMineList[i].transform.position, Quaternion.identity);
            
            //ADD LIST Mine Area
            toxicMineAreaList.Add(toxicMineAreaObject);
        }
        
        foreach (var VARIABLE in toxicMineAreaList)
        {
            Destroy(VARIABLE, 1f); //destroy toxic mine area
        }
        
        for (int i = 0; i < numberOfToxicMines; i++)
        {
            Destroy(toxicMineList[i].gameObject, 1f);
        }
        
        //Clear toutes les list 
        toxicMineAreaWarningList.Clear();
        toxicMineAreaList.Clear();
        toxicMineList.Clear();

        yield return new WaitForSeconds(1f);

        if (currentToxicMineAmount > 0)
        {
            StartCoroutine(ToxicMine());
        }
        else if(currentToxicMineAmount == 0)
        { 
            //var nextState = lastStatesList[Random.Range(0, lastStatesList.Count)];
            SwitchState(ThrowingState);
        }
    }

    

    #endregion

    #region ThrowingState

    public void ThrowingManager()
    {
        StartCoroutine(Throwing());
        Debug.Log($"<color=red>THROWING STATE HAS BEGUN</color>");

    }

    private IEnumerator Throwing()
    {
        currentThrowAmount--;
        yield return new WaitForSeconds(1f);

        var posBossBeforeSpawn = transform.position; //get la pos du boss 
        var slugObject = Instantiate(slug, new Vector2(posBossBeforeSpawn.x + Random.Range(-2f, 2f),posBossBeforeSpawn.y +  Random.Range(-2f, 2f)), Quaternion.identity);

        yield return new WaitForSeconds(8f);
        
        if (currentThrowAmount > 0)
        {
            StartCoroutine(Throwing());
        }
        else if(currentThrowAmount == 0)
        { 
            //var nextState = lastStatesList[Random.Range(0, lastStatesList.Count)];
            SwitchState(ToxicMineState);
        }
    }

    #endregion

    #region BoxingState

    public void BoxingManager()
    {
        StartCoroutine(Boxing());
        Debug.Log($"<color=red>BOXING STATE HAS BEGUN</color>");

    }

    private IEnumerator Boxing()
    {
        
        yield return new WaitForSeconds(1f);
    }

    #endregion
}
