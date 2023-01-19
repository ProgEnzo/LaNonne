using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Controller;
using DG.Tweening;
using Manager;
using Pathfinding;
using Shop;
using Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AI.Boss
{
    public class BossStateManager : MonoBehaviour
    {
        private BossBaseState currentState;
        private readonly BossDashingState dashingState = new();
        private readonly BossThrowingState throwingState = new();
        private readonly BossAttackCircleState attackCircleState = new();
        private readonly BossVacuumState vacuumState = new();
        private readonly BossToxicMineState toxicMineState = new();
        private readonly BossTransitionState transitionState = new();
        private readonly BossBoxingState boxingState = new();
        private readonly BossSpawnState spawnState = new();

        [SerializeField] private Rigidbody2D rb;
        private PlayerController player;
        [SerializeField] private AIPath bossAI;
        [SerializeField] private AIDestinationSetter aiDestinationSetter;
        [SerializeField] private GameObject bossPuppet;
        [SerializeField] private GameObject chrysalisPuppet;
        [SerializeField] private BoxCollider2D bossBoxCollider;
        [SerializeField] private CapsuleCollider2D bossCapsuleCollider;
        [SerializeField] private CapsuleCollider2D chrysalisCapsuleCollider;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip throwAnimation;
    
        //LIST
        private readonly List<BossBaseState> firstStatesList = new();
        private readonly List<BossBaseState> lastStatesList = new();
    
        public List<GameObject> spawnerList = new();

        [Header("Overall Stats")]
        private Image hpBossBar;
        public int currentHealth;
        public int maxHealth;
        public float playerNormalSpeed;
        public float bossNormalSpeed;

        [Header("Virtual Camera")]
        private CamManager camManager;

        [Header("----Dash----")] 
        public GameObject dashMine;
        public GameObject dashWarning;
        public int bodyDamage;
    
        public float dashPower;
        public float timeBeforeDashing;
        public float waitEndDash;
        
        public bool canShakeOnWallBump;

        public int dashAmount;
        public int currentDashAmount;
        public int numberOfMines;

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
        public int vacuumAmount;
        public int currentVacuumAmount;
        
        [Header("----TransitionState----")] 
        private GameObject shockwaveGameObject;

        public bool takingDamage = true;
        public int numberOfSpawn;
        public float transitionCooldown;


        [Header("----ToxicMineState----")]
        public List<GameObject> toxicMineList = new();
        public List<GameObject> toxicMineAreaWarningList = new();
        public List<GameObject> toxicMineAreaList = new();

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

        [Header("----BoxingState----")] 
        public List<GameObject> circleBoxingWarningObjectList = new();
        public List<GameObject> circleBoxingObjectList = new();

        public GameObject circleBoxing;
        public GameObject circleBoxingWarning;
        public float distanceBetweenPlayer;
        public float aggroBoxingRange;

        public float currentDamageTaken;
        public float maxDamageTaken;
    
        public float numberOfBoxingCircleWarning;
        public float numberOfBoxingCircle;

        [Header("----StackSystem----")]
        internal readonly (EffectManager.Effect effect, int level)[] stacks = new (EffectManager.Effect, int)[3];
        [SerializeField, ShowOnly] internal float[] stackTimers = new float[3];
        [SerializeField, ShowOnly] internal bool[] areStacksOn = new bool[3];
        private EffectManager effectManager;
        [SerializeField, ShowOnly] internal float aiPathSpeed;
        [SerializeField, ShowOnly] internal float currentAiPathSpeed;
        [SerializeField, ShowOnly] internal float currentVelocitySpeed;
        [SerializeField, ShowOnly] internal float currentDamageMultiplier;
        //[SerializeField, ShowOnly] internal float currentEpDropMultiplier;
        
        [Header("----HitStopAndKnockBack----")]
        private Coroutine currentHitStopCoroutine;
        private float lastVelocitySpeed;
        private static readonly int BossAnimState = Animator.StringToHash("bossAnimState");


        private void Start()
        {
            hpBossBar = GameObject.FindGameObjectWithTag("Boss HealthBar").transform.GetChild(0).GetComponent<Image>();
            player = PlayerController.instance;
            effectManager = EffectManager.instance;
            camManager = CamManager.instance;
            aiDestinationSetter.target = PlayerController.instance.transform;
            shockwaveGameObject = GameObject.Find("Shockwave");
            
            currentState = dashingState; //starting state for the boss state machine
            currentState.EnterState(this); //"this" is this Monobehavior script
        
            //HEALTH
            currentHealth = maxHealth;
            currentDamageMultiplier = 1f;
            hpBossBar.fillAmount = 0f;
            hpBossBar.DOFillAmount((float)currentHealth / maxHealth, 1f);

            //MOVEMENT SPEED
            player.soController.moveSpeed = playerNormalSpeed;
            currentAiPathSpeed = bossNormalSpeed;
            aiPathSpeed = currentAiPathSpeed;
            currentVelocitySpeed = dashPower;

            //STATES
            firstStatesList.Add(dashingState);
            firstStatesList.Add(attackCircleState);
            firstStatesList.Add(vacuumState);
            firstStatesList.Add(spawnState);
        
            lastStatesList.Add(throwingState);
            lastStatesList.Add(toxicMineState);
            lastStatesList.Add(spawnState);

            //VIRTUAL CAMERA
            GameObject.Find("vCamPlayer").GetComponent<CinemachineVirtualCamera>();
            GameObject.Find("vCamPlayerShake").GetComponent<CinemachineVirtualCamera>();
            GameObject.Find("vCamBoss").GetComponent<CinemachineVirtualCamera>();
            GameObject.Find("vCamBossShake").GetComponent<CinemachineVirtualCamera>();

            //STACKS
            for (var i = 0; i < stacks.Length; i++)
            {
                stacks[i].effect = EffectManager.Effect.None; 
                stacks[i].level = 0;
                stackTimers[i] = 0;
                areStacksOn[i] = false;
            }
        }

        private void Update()
        {
            bossAI.maxSpeed = aiPathSpeed;

            var position = transform.position;
            var playerPosition = player.transform.position;
            distanceBetweenPlayer = Vector2.Distance(new Vector2(position.x, position.y), new Vector2(playerPosition.x, playerPosition.y));

            CheckDirection();
            EffectCheck();
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                player.TakeDamage(bodyDamage);
            }

            //Verif pour le Dashing State
            if (col.gameObject.CompareTag("BossWall"))
            {
                if (canShakeOnWallBump)
                {
                    StartCoroutine(camManager.ShakeCam());
                    rb.velocity = Vector2.zero;
                    canShakeOnWallBump = false;
                }
            }
        }

        private void SwitchState(BossBaseState state)
        {
            Reinit();
            currentState = state;
            state.EnterState(this);
        }

        private void Reinit()
        {
            currentDashAmount = dashAmount;
            currentAttackCircleAmount = attackCircleAmount;
            currentVacuumAmount = vacuumAmount;

            currentToxicMineAmount = toxicMineAmount;
            currentThrowAmount = throwAmount;
        }

        /*private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, aggroBoxingRange);
        }*/

        #region Health Boss

        public void TakeDamageOnBossFromPlayer(int damage)
        {
            if (takingDamage)
            {
                currentHealth -= (int)(damage * currentDamageMultiplier);
                hpBossBar.DOFillAmount((float)currentHealth / maxHealth, 0.1f);

                currentDamageTaken += damage;
                
            }
        
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        #endregion

        #region HitStopAndKnockBack

        internal void HitStopAndKnockBack(float hitStopDuration, float knockBackForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * knockBackForce, ForceMode2D.Impulse);
            if (currentHitStopCoroutine != null)
            {
                StopCoroutine(currentHitStopCoroutine);
                currentVelocitySpeed = lastVelocitySpeed;
            }
            currentHitStopCoroutine = StartCoroutine(HitStop(hitStopDuration));
        }
        
        private IEnumerator HitStop(float hitStopDuration)
        {
            lastVelocitySpeed = currentVelocitySpeed;
            bossAI.canMove = false;
            currentVelocitySpeed = 0;
            yield return new WaitForSeconds(hitStopDuration);
            currentVelocitySpeed = lastVelocitySpeed;
            bossAI.canMove = true;
            currentHitStopCoroutine = null;
        }

        #endregion

        #region StackSystem
        
        private void EffectCheck()
        {
            for (var i = 0; i < stacks.Length; i++)
            {
                if (stacks[i].effect == EffectManager.Effect.None)
                {
                    continue;
                }
                stackTimers[i] -= Time.deltaTime;
                if (stackTimers[i] <= 0)
                {
                    stackTimers[i] = 0;
                    stacks[i].effect = EffectManager.Effect.None;
                    stacks[i].level = 0;
                    continue;
                }

                if (areStacksOn[i]) continue;
                areStacksOn[i] = true;
                effectManager.EffectSwitch(stacks[i].effect, stacks[i].level, gameObject, i);
            }
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
            canShakeOnWallBump = true;
            currentDashAmount--; //décrémente de 1 le nombre de dash restant
            aiPathSpeed = 0;
            
            //DO THE WARNING HERE
            var position = transform.position;
            var playerPosition = player.transform.position;
            Vector2 direction = playerPosition - position;
            var dashWarningObject = Instantiate(dashWarning, position, Quaternion.identity);
            dashWarningObject.transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.right, playerPosition - dashWarningObject.transform.position), 0f); //Rotate warning to the player
            yield return new WaitForSeconds(timeBeforeDashing);

            Destroy(dashWarningObject);
            aiDestinationSetter.enabled = false;
            bossAI.enabled = false;
            
            animator.SetInteger(BossAnimState, 1);

            rb.drag = 0f;
            rb.angularDrag = 0.05f;
            rb.velocity = direction.normalized * currentVelocitySpeed; // DO DASH

            //HOW MANY MINES DURING DASH
            for (var i = 0; i < numberOfMines; i++)
            {
                StartCoroutine(DashMine());
                yield return new WaitForSeconds(0.4f);
            }

            aiPathSpeed = currentAiPathSpeed;
            aiDestinationSetter.enabled = true;
            bossAI.enabled = true;
            
            animator.SetInteger(BossAnimState, 0);
            rb.drag = 10f;
            rb.angularDrag = 10f;
            
            switch (currentDashAmount)
            {
                case > 0 when currentHealth >= maxHealth / 2:
                    StartCoroutine(Dash());
                    break;
                case 0:
                {
                    aiPathSpeed = 0;
                    yield return new WaitForSeconds(waitEndDash);
                
                    aiPathSpeed = currentAiPathSpeed;
                    var nextState = firstStatesList[Random.Range(0, firstStatesList.Count)];
            
                    SwitchState(nextState);
                    break;
                }
                default:
                {
                    if (currentHealth <= maxHealth / 2)
                    {
                        SwitchState(transitionState);
                    }

                    break;
                }
            }
        }

        private IEnumerator DashMine()
        {
            var dashMineObject = Instantiate(dashMine, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        
            dashMineObject.transform.DOScale(new Vector2(3, 3), 1.5f);
            Destroy(dashMineObject, 1.5f);
            yield return new WaitForSeconds(0.3f);
        }

        #endregion

        #region AttackCircleState

        public void AttackCircleManager()
        {
            StartCoroutine(AttackCircle());
            var posBossBeforeSpawn = transform.position; //get la pos du boss
            Instantiate(spawnerList[Random.Range(0, spawnerList.Count)], new Vector2(posBossBeforeSpawn.x + Random.Range(-2f, 2f),posBossBeforeSpawn.y +  Random.Range(-2f, 2f)), Quaternion.identity);

            Debug.Log($"<color=green>ATTACK CIRCLE STATE HAS BEGUN</color>");
        }
    
        private IEnumerator AttackCircle()
        {
            aiPathSpeed = 0;
            currentAttackCircleAmount--;
            yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
            rb.bodyType = RigidbodyType2D.Static;
            var circleObjectWarning = Instantiate(attackCircleWarning, player.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(attackCircleSpacingCooldown);
        
            Destroy(circleObjectWarning);
            var circleObject = Instantiate(attackCircle, circleObjectWarning.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(attackCircleSpacingCooldown);
            
            Destroy(circleObject);
            rb.bodyType = RigidbodyType2D.Dynamic;

            switch (currentAttackCircleAmount)
            {
                case > 0 when currentHealth >= maxHealth / 2:
                    StartCoroutine(AttackCircle());
                    break;
                case 0:
                {
                    var nextState = firstStatesList[Random.Range(0, firstStatesList.Count)];
            
                    SwitchState(nextState);
                    break;
                }
                default:
                {
                    if (currentHealth <= maxHealth / 2)
                    {
                        aiPathSpeed = currentAiPathSpeed;
                        SwitchState(transitionState);
                    }

                    break;
                }
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
            aiPathSpeed = 1f; // decrease speed
            yield return new WaitForSeconds(2f);

            aiPathSpeed = 0f; // STOP THE BOSS
        
            var bossPos = transform.position;

            //VACUUM AREA
            var vacuumGameObject = Instantiate(vacuumArea, bossPos, Quaternion.identity);
            vacuumGameObject.transform.parent = gameObject.transform; //set le prefab en child
            vacuumParticle.SetActive(true);
        
            //TOXIC AREA
            var toxicAreaObject = Instantiate(toxicArea, bossPos, Quaternion.identity);
            toxicAreaObject.transform.DOScale(new Vector2(3.5f, 3.5f), 2f);
            
            rb.bodyType = RigidbodyType2D.Static;
            animator.SetInteger(BossAnimState, 3);
            
            yield return new WaitForSeconds(vacuumCooldown);
            
            animator.SetInteger(BossAnimState, 0);
            rb.bodyType = RigidbodyType2D.Dynamic;

            Destroy(vacuumGameObject);
            vacuumParticle.SetActive(false);
            Destroy(toxicAreaObject);
            aiPathSpeed = currentAiPathSpeed; //Change the speed back to normal

            switch (currentVacuumAmount)
            {
                case > 0 when currentHealth >= maxHealth / 2:
                    StartCoroutine(Vacuum());
                    break;
                case 0:
                {
                    var nextState = firstStatesList[Random.Range(0, firstStatesList.Count)];
            
                    SwitchState(nextState);
                    break;
                }
                default:
                {
                    if (currentHealth <= maxHealth / 2)
                    {
                        aiPathSpeed = currentAiPathSpeed;
                        SwitchState(transitionState);
                    }

                    break;
                }
            }
        }
        
        #endregion

        #region SpawnEnemyState

        public void SpawnEnemyManager()
        {
            StartCoroutine(SpawnEnemy());
        }
        
        private IEnumerator SpawnEnemy()
        {
            var posBossBeforeSpawn = transform.position; //get la pos du boss
            Instantiate(spawnerList[Random.Range(0, spawnerList.Count)], new Vector2(posBossBeforeSpawn.x + Random.Range(-2f, 2f),posBossBeforeSpawn.y +  Random.Range(-2f, 2f)), Quaternion.identity);
            yield return new WaitForSeconds(1f);

            if (currentHealth >= maxHealth / 2)
            {
                var nextStateFirst = firstStatesList[Random.Range(0, firstStatesList.Count)];
                SwitchState(nextStateFirst);
            }
            else
            {
                var nextStateLast = lastStatesList[Random.Range(0, lastStatesList.Count)];
                SwitchState(nextStateLast);
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
            aiPathSpeed = 0;
            player.enabled = false;
            player.transform.GetChild(0).gameObject.SetActive(false);
            //player.soController.moveSpeed = 0;
            yield return new WaitForSeconds(1f);
        
            camManager.ChangeBossCamState(1); //on laisse la priorité à la vCam du boss
            takingDamage = false;
            yield return new WaitForSeconds(1f);

            camManager.ChangeBossCamState(2); //Boss lâche un cri qui annonce la transition
            rb.bodyType = RigidbodyType2D.Static;
            animator.SetInteger(BossAnimState, 3);
            yield return new WaitForSeconds(2f); //temps du cri
            
            camManager.ChangeBossCamState(1); //on redonne la priorité à la vCam du BOSS
            bossPuppet.SetActive(false);
            chrysalisPuppet.SetActive(true);
            bossBoxCollider.enabled = false;
            bossCapsuleCollider.enabled = false;
            chrysalisCapsuleCollider.enabled = true;
            animator.SetInteger(BossAnimState, 0);
            yield return new WaitForSeconds(1f); //transition BOSS to player
            
            camManager.ChangeBossCamState(0);
            player.enabled = true;
            player.transform.GetChild(0).gameObject.SetActive(true);
            //player.soController.moveSpeed = 40f;
            yield return new WaitForSeconds(3f);

            for (var i = 0; i < numberOfSpawn; i++)
            {
                yield return new WaitForSeconds(0.3f);
            
                var posBossBeforeSpawn = transform.position; //get la pos du boss
                Instantiate(spawnerList[Random.Range(0, spawnerList.Count)], new Vector2(posBossBeforeSpawn.x + Random.Range(-2f, 2f),posBossBeforeSpawn.y +  Random.Range(-2f, 2f)), Quaternion.identity);
            }
            yield return new WaitForSeconds(transitionCooldown);
            bossPuppet.SetActive(true);
            chrysalisPuppet.SetActive(false);
            bossBoxCollider.enabled = true;
            bossCapsuleCollider.enabled = true;
            chrysalisCapsuleCollider.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        
            //CODE FOR THE EXPLOSION AFTER THE TRANSITION HERE
            shockwaveGameObject.transform.DOScale(new Vector2(10f, 10f), 1f);
            Debug.Log("AFEPIAEPFIAEPFIAJEPFAFJEPAEJFPAEFJPAFJPAFJE");
            yield return new WaitForSeconds(1f);

            Destroy(shockwaveGameObject);
            aiPathSpeed = currentAiPathSpeed;
            takingDamage = true;
            SwitchState(throwingState);
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
            for (var i = 0; i < numberOfToxicMines; i++)
            {
                var posBossBeforeSpawn = transform.position; //get la pos du boss 
            
                toxicMineObject = Instantiate(toxicMine, posBossBeforeSpawn, Quaternion.identity); //spawn toxic mines
                toxicMineObject.transform.DOMove(new Vector2(posBossBeforeSpawn.x + Random.Range(-7f, 7f),posBossBeforeSpawn.y +  Random.Range(-7f, 7f)), 1.5f).SetEase(Ease.OutQuint); //send toxic mines everywhere
            
                toxicMineList.Add(toxicMineObject); //add toxic mines into the list
            }
        
            yield return new WaitForSeconds(1.5f);

            //SPAWN TOXIC MINE AREA WARNING
            for (var i = 0; i < numberOfToxicMines; i++)
            {
                var toxicMineAreaWarningObject = Instantiate(toxicMineWarning, toxicMineList[i].transform.position, Quaternion.identity);
            
                //ADD LIST Mine Area Warning
                toxicMineAreaWarningList.Add(toxicMineAreaWarningObject);
            }
        
            foreach (var variable in toxicMineAreaWarningList)
            {
                Destroy(variable, 1f); //destroy toxic mine area warning
            }
        
            yield return new WaitForSeconds(1f);

            //SPAWN TOXIC MINE AREA
            for (var i = 0; i < numberOfToxicMines; i++)
            {
                var toxicMineAreaObject = Instantiate(toxicMineArea, toxicMineList[i].transform.position, Quaternion.identity);
            
                //ADD LIST Mine Area
                toxicMineAreaList.Add(toxicMineAreaObject);
            }
        
            foreach (var variable in toxicMineAreaList)
            {
                Destroy(variable, 1f); //destroy toxic mine area
            }
        
            for (var i = 0; i < numberOfToxicMines; i++)
            {
                Destroy(toxicMineList[i].gameObject, 1f); //destroy toxic mine
            }
        
            //Clear toutes les list 
            toxicMineAreaWarningList.Clear();
            toxicMineAreaList.Clear();
            toxicMineList.Clear();
            yield return new WaitForSeconds(1f);
            
            if(distanceBetweenPlayer < aggroBoxingRange && currentDamageTaken > maxDamageTaken)
            {
                SwitchState(boxingState);
            }
            else switch (currentToxicMineAmount)
            {
                case > 0:
                    StartCoroutine(ToxicMine());
                    break;
                case 0:
                {
                    var nextState = lastStatesList[Random.Range(0, lastStatesList.Count)];
                    SwitchState(nextState);
                    break;
                }
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
            yield return new WaitForSeconds(2f - throwAnimation.length * 0.9f);
            
            rb.bodyType = RigidbodyType2D.Static;
            StartCoroutine(ChangeAnimation(4));
            yield return new WaitForSeconds(throwAnimation.length * 0.9f);

            var posBossBeforeSpawn = transform.position; //get la pos du boss 
            Instantiate(slug, new Vector2(posBossBeforeSpawn.x,posBossBeforeSpawn.y), Quaternion.identity);
            yield return new WaitForSeconds(throwAnimation.length * 0.1f);

            rb.bodyType = RigidbodyType2D.Dynamic;
            yield return new WaitForSeconds(3f - throwAnimation.length * 0.1f);
            
            if (distanceBetweenPlayer < aggroBoxingRange && currentDamageTaken > maxDamageTaken)
            {
                SwitchState(boxingState);
            }
            else switch (currentThrowAmount)
            {
                case > 0:
                    StartCoroutine(Throwing());
                    break;
                case 0:
                {
                    var nextState = lastStatesList[Random.Range(0, lastStatesList.Count)];
                    SwitchState(nextState);
                    break;
                }
            }
        }

        #endregion

        #region BoxingState
    
        public void BoxingManager()
        {
            StartCoroutine(Boxing());
            Debug.Log($"<color=red>COROUTINE Boxing HAS begun</color>");
        }

        private IEnumerator Boxing()
        {
            //yield return new WaitForSeconds(1f);

            if (currentDamageTaken > maxDamageTaken)
            {
                animator.SetInteger(BossAnimState, 2);
                rb.bodyType = RigidbodyType2D.Static;
                
                for (var i = 0; i < numberOfBoxingCircleWarning; i++)
                {
                    var posPlayerBeforeSpawn = player.transform.position; //get la pos du boss
                    var circleBoxingWarningObject = Instantiate(circleBoxingWarning, new Vector2(posPlayerBeforeSpawn.x,posPlayerBeforeSpawn.y), Quaternion.identity);
                    
                    circleBoxingWarningObjectList.Add(circleBoxingWarningObject);
                    yield return new WaitForSeconds(0.2f);
                }

                for (var i = 0; i < numberOfBoxingCircle; i++)
                {
                    var circleBoxingObject = Instantiate(circleBoxing, circleBoxingWarningObjectList[i].transform.position, Quaternion.identity);
                    StartCoroutine(camManager.ShakeCam()); //DO SHAKE CAMERA

                    //ADD to the list 
                    circleBoxingObjectList.Add(circleBoxingObject);
                    yield return new WaitForSeconds(0.2f);
                }

                foreach (var variableWarning in circleBoxingWarningObjectList)
                {
                    Destroy(variableWarning);
                }

                foreach (var variable in circleBoxingObjectList)
                {
                    Destroy(variable);
                }
                
                //Clear toutes les list
                circleBoxingWarningObjectList.Clear();
                circleBoxingObjectList.Clear();
                
                currentDamageTaken = 0;
                animator.SetInteger(BossAnimState, 0);
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            var nextState = lastStatesList[Random.Range(0, lastStatesList.Count)];
            SwitchState(nextState);
        }

        #endregion

        #region Animator

        private IEnumerator ChangeAnimation(int state)
        {
            animator.SetInteger(BossAnimState, state);
            yield return new WaitForNextFrameUnit();
            animator.SetInteger(BossAnimState, 0);
        }
        
        private void CheckDirection()
        {
            var puppetLocalScale = bossPuppet.transform.localScale;
            if (rb.velocity.x != 0)
            {
                bossPuppet.transform.localScale = new Vector3(MathF.Sign(rb.velocity.x) * MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
            else
            {
                bossPuppet.transform.localScale = player.transform.position.x > transform.position.x ? new Vector3(MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z) : new Vector3(-MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
        }

        #endregion
    }
}
