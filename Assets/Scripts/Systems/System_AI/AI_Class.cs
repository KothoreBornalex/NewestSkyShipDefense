using MoreMountains.Feedbacks;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static IStatistics;
using UnityEngine.Events;

public class AI_Class : MonoBehaviour, IStatistics
{
    public enum FactionsEnum
    {
        Orc = 0,
        Elf = 1,
        Necromancer = 2,
        Human = 3
    }
    public enum SoldiersEnum
    {
        Larbin_A = 0,
        Larbin_B = 1,
        Larbin_C = 2,
        LittleChief = 3,
        Chief = 4,
        BigChief = 5,
    }


    [Button("Receive Damage")] void Attack() => LoseLP();
    private void LoseLP()
    {
        DecreaseStat(StatName.Health, (int)UnityEngine.Random.Range(1, 3));
    }


    [Header("Global AI Fields")]
    [SerializeField] private bool _isAlive;
    [SerializeField] private SoldiersEnum _unitType;
    [SerializeField, Expandable] private AI_Data _ai_Data;
    [SerializeField] private bool _setChase;
    private int _objectifID;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private Material _material;
    private Animator _animator;
    private PooledObject _pooledObject;

    [SerializeField] private WeaponsScriptableObject _weaponsList;
    private List<Statistics> _aiStatistics = new List<Statistics>();

    [Header("Pathfinding Fields")]
    private NavMeshAgent _navMeshAgent;

    [Header("Patrols Fields")]
    [SerializeField] private Transform[] patrolWayPoints;
    private int currentPatrolPoint;


    [Header("Attack Fields")]
    private int _currentWeaponIndex;
    [SerializeField, Range(0, 5)] private float _attackFrequency;
    [SerializeField] private Transform _bulletSpawnPoint;
    private float _attackTimer;

    [Header("FeedBacks Set")]
    [SerializeField] private MMFeedbacks _hurtFeedBack;

    //Death Fields
    private float currentTimeSinceDeath;

    //Animations Hash
    private int hash_Reset = Animator.StringToHash("Reset");
    private int hash_Attack = Animator.StringToHash("Attack");
    private int hash_GetHit = Animator.StringToHash("GetHit");
    private int hash_isDead = Animator.StringToHash("isDead");
    private int hash_isWalking = Animator.StringToHash("isWalking");

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _currentWeaponIndex = GetWeaponIndex(_unitType);
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _material = _skinnedMeshRenderer.material;
        _animator = GetComponentInChildren<Animator>();
        _pooledObject = GetComponent<PooledObject>();
    }

    private void Start()
    {
        _objectifID = Random.Range(0, GameManager.instance.Objectifs.Length);
    }


    private void Update()
    {
        if (_isAlive)
        {
            HandleChase();

            if (_skinnedMeshRenderer.sharedMaterial.color != Color.white)
            {
                _material.color = Vector4.Lerp(_material.color, Color.white, Time.deltaTime * 3.0f);
            }
        }
        else
        {
            if (_skinnedMeshRenderer.sharedMaterial.color != Color.black)
            {
                _material.color = Vector4.Lerp(_material.color, Color.black, Time.deltaTime * 3.0f);
            }


            transform.Translate(-transform.up * Time.deltaTime * 0.3f);
            currentTimeSinceDeath += Time.deltaTime;

            if (currentTimeSinceDeath >= 3.0f)
            {
                AISpawner_Manager.instance.UnSpawn(_pooledObject);
            }

        }

    }

    #region Global AI Functions

    public void InitializedAI()
    {
        Debug.Log("Initialized AI Started");
        _isAlive = true;

        InitializeStats();
        FreezPhysics();

        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;


        _animator.SetBool(hash_isDead, false);
        _animator.SetBool(hash_isWalking, false);

        _capsuleCollider.enabled = true;

        _material.color = Color.white;
    }

    public void FreezPhysics()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnFreezPhysics()
    {
        _rigidbody.constraints = RigidbodyConstraints.None;
    }

    public void Death()
    {
        Instantiate<GameObject>(_ai_Data.DeathObject, transform.position, Quaternion.identity);
        _isAlive = false;
        _animator.SetBool(hash_isDead, true);
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        _navMeshAgent.enabled = false;

        _capsuleCollider.enabled = false;

        GameManager.instance.UnitsDeadThisRound++;
    }

    #endregion


    #region Patrol & Chases Functions
    public void HandleChase()
    {
        HandleObjectivesChoice();

        if (!_setChase)
        {
            _navMeshAgent.SetDestination(GameManager.instance.Objectifs[_objectifID].transform.position);
            _setChase = true;
        }


        if (Vector3.Distance(transform.position, GameManager.instance.Objectifs[_objectifID].transform.position) > _ai_Data.AttackRange && !_navMeshAgent.hasPath)
        {
            _navMeshAgent.SetDestination(GameManager.instance.Objectifs[_objectifID].transform.position);
        }
        
        if(Vector3.Distance(transform.position, GameManager.instance.Objectifs[_objectifID].transform.position) <= _ai_Data.AttackRange)
        {
            if (_animator.GetBool(hash_isWalking))
            {
                _animator.SetBool(hash_isWalking, false);
            }

            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();

            _attackTimer += Time.deltaTime;
            HandleAIAttack();
        }
        else if(!_navMeshAgent.hasPath)
        {
            _navMeshAgent.SetDestination(GameManager.instance.Objectifs[_objectifID].transform.position);
            _animator.SetBool(hash_isWalking, true);
        }
    }

    public void HandlePatrol()
    {
        if(patrolWayPoints.Length == 0)
        {
            return;
        }

        if (!_navMeshAgent.hasPath)
        {
            _navMeshAgent.SetDestination(patrolWayPoints[currentPatrolPoint].transform.position);
        }


        if (Mathf.Round(transform.position.x) == Mathf.Round(patrolWayPoints[currentPatrolPoint].position.x) && Mathf.Round(transform.position.y) == Mathf.Round(patrolWayPoints[currentPatrolPoint].position.y) && currentPatrolPoint != patrolWayPoints.Length)
        {
            currentPatrolPoint++;
            _navMeshAgent.SetDestination(patrolWayPoints[currentPatrolPoint].transform.position);

        }
        else if(patrolWayPoints.Length == currentPatrolPoint)
        {
            currentPatrolPoint = 0;
            _navMeshAgent.SetDestination(patrolWayPoints[currentPatrolPoint].transform.position);
        }
    }


    public void HandleObjectivesChoice()
    {
        if (GameManager.instance.Objectifs[_objectifID].ObjectScript.GetObjectState() == IObjects.ObjectStates.Destroyed)
        {
            if (GameManager.instance.ObjectiveExist())
            {
                _objectifID = GameManager.instance.GetObjectif();
            }
        }
    }

    
    #endregion


    #region Global Attacks Functions
    public void HandleAIAttack()
    {
        //Debug.Log("AI Attack !!");

        if (_attackTimer >= _weaponsList.WeaponsList[_currentWeaponIndex].weaponCoolDown)
        {
            _animator.SetTrigger(hash_Attack);

            switch (_unitType)
            {
                case SoldiersEnum.Larbin_A:
                        LarbinA_Attack();
                    break;


                case SoldiersEnum.Larbin_B:
                        HandlePistolAttack();
                    break;

                case SoldiersEnum.Larbin_C:
                        HandleFusilAttack();
                    break;

            }

            _attackTimer = 0;
        }

    }


    public void LarbinA_Attack()
    {
        Debug.Log("Katana Attack !!");
        //AudioManager.instance.PlayOneShot_GlobalSound(FMODEvents.instance.Weapons_KatanaSlash);
        GameManager.instance.Objectifs[_objectifID].DecreaseStat(StatName.Health, _weaponsList.WeaponsList[_currentWeaponIndex].weaonDamage);
    }


    public void HandlePistolAttack()
    {
        int currentWeapon = GetWeaponIndex(_unitType);

        //AudioManager.instance.PlayOneShot_GlobalSound(FMODEvents.instance.Weapons_PistolShot);


        // Calculate the rotation of the pistol in degrees
        float pistolRotation = _bulletSpawnPoint.eulerAngles.z;
        float pistolRotationRad = pistolRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(pistolRotationRad), Mathf.Sin(pistolRotationRad));

        //Instantiation Projectile
        //WeaponContactTrigger projectile = Instantiate<GameObject>(PlayerStateMachine.instance.WeaponsList.WeaponsList[currentWeapon].attackProjectile, transform.position, Quaternion.identity).GetComponent<WeaponContactTrigger>();
        //projectile.direction = direction;
        //projectile.TargetedFaction = Factions.Player;


    }

    public void HandleFusilAttack()
    {
        int currentWeapon = GetWeaponIndex(_unitType);

        //AudioManager.instance.PlayOneShot_GlobalSound(FMODEvents.instance.Weapons_FusilShot);


        // Calculate the rotation of the pistol in degrees
        float pistolRotation = _bulletSpawnPoint.eulerAngles.z;
        float pistolRotationRad = pistolRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(pistolRotationRad), Mathf.Sin(pistolRotationRad));

        //Instantiation Projectile
        //WeaponContactTrigger projectile = Instantiate<GameObject>(PlayerStateMachine.instance.WeaponsList.WeaponsList[currentWeapon].attackProjectile, transform.position, Quaternion.identity).GetComponent<WeaponContactTrigger>();
        //projectile.direction = direction;
        //projectile.TargetedFaction = Factions.Player;

    }

    public int GetWeaponIndex(SoldiersEnum soldierEnum)
    {
        for (int i = 0; i < _weaponsList.WeaponsList.Count; i++)
        {
            if (_weaponsList.WeaponsList[i].soldierType == soldierEnum)
            {
                return i;
            }
        }

        return 0;
    }

    #endregion



    #region IStatistics Functions

    public void InitializeStats()
    {
        foreach (Statistics statistics in _ai_Data.AiStatistics)
        {
            _aiStatistics.Add(new Statistics(statistics._statName, statistics._statCurrentValue, statistics._statMaxValue));
        }
    }


    public void SetStat(StatName statName, float statValue)
    {

        foreach (Statistics stats in _aiStatistics)
        {
            if (stats._statName == statName)
            {
                stats._statCurrentValue = statValue;
            }
        }

    }

    public void DecreaseStat(StatName statName, float decreasingValue)
    {

        foreach (Statistics stats in _aiStatistics)
        {
            if (stats._statName == statName)
            {
                if (stats._statName == StatName.Health)
                {
                    //AudioManager.instance.PlayOneShot_GlobalSound(FMODEvents.instance.Player_Hurt);

                    if (stats._statCurrentValue <= 0 && _isAlive)
                    {
                        Death();
                    }
                    else
                    {
                        _material.color = Color.red;
                        _animator.SetTrigger(hash_GetHit);
                        _hurtFeedBack?.PlayFeedbacks();
                    }
                }

                stats._statCurrentValue -= decreasingValue;
                stats._statCurrentValue = Mathf.Clamp(stats._statCurrentValue, 0, stats._statMaxValue);
                return;
            }
        }

    }

    public void IncreaseStat(StatName statName, float increasingValue)
    {

        foreach (Statistics stats in _aiStatistics)
        {
            if (stats._statName == statName)
            {
                if (stats._statName == StatName.Health)
                {
                    //AudioManager.instance.PlayOneShot_GlobalSound(FMODEvents.instance.Player_Healed);
                    //_aiSprite.color = Color.green;
                }

                stats._statCurrentValue += increasingValue;
                stats._statCurrentValue = Mathf.Clamp(stats._statCurrentValue, 0, stats._statMaxValue);
                return;
            }
        }

    }



    public void ResetStat(StatName statName)
    {
        foreach (Statistics stat in _aiStatistics)
        {
            if (stat._statName == statName)
            {
                stat._statCurrentValue = stat._statMaxValue;
                return;
            }
        }
    }

    public float GetStat(StatName statName)
    {
        foreach (Statistics stats in _aiStatistics)
        {
            if (stats._statName == statName)
            {
                return stats._statCurrentValue;
            }
        }

        return 0.0f;
    }


    public float GetMaxStat(StatName statName)
    {
        foreach (Statistics stats in _aiStatistics)
        {
            if (stats._statName == statName)
            {
                return stats._statMaxValue;
            }
        }

        return 0.0f;
    }
    #endregion
}
