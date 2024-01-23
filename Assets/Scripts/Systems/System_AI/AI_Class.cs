using MoreMountains.Feedbacks;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static IStatistics;
using UnityEngine.Events;
using static ScriptableObject_AnimationsBank;

public class AI_Class : MonoBehaviour, IStatistics
{
    public enum FactionsEnum
    {
        Orc = 0,
        Elf = 1,
        Necromancer = 2,

        FactionsCount = 3,

        Human = 4,
    }
    public enum SoldiersEnum
    {
        Infantry = 0,
        Scout = 1,
        Cavalry = 2,
        MiniBoss = 3,

        UnitsTypesCount = 4
    }


    [Button("Receive Damage")] void Attack() => LoseLP();
    private void LoseLP()
    {
        DecreaseStat(StatName.Health, (int)UnityEngine.Random.Range(1, 3));
    }


    [Header("Global AI Fields")]
    [SerializeField] private bool _isAlive;
    [SerializeField] private SoldiersEnum _unitType;
    [SerializeField] private FactionsEnum _faction;
    [SerializeField] private Units_Data_Bank _unitsDataBank;
    [SerializeField, Expandable] private Unit_Data _unitData;
    [SerializeField] private bool _setChase;
    [SerializeField] private ParticleSystem _deathEffect;

    private int _objectifID;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Material _material;
    private CustomAnimator _customAnimator;
    private PooledObject _pooledObject;

    [SerializeField] private WeaponsScriptableObject _weaponsList;
    private List<Statistics> _aiStatistics = new List<Statistics>();

    [Header("Pathfinding Fields")]
    private NavMeshAgent _navMeshAgent;
    private float _agentRadius;
    private float _capsuleRadius;

    [Header("Patrols Fields")]
    [SerializeField] private Transform[] patrolWayPoints;
    private int currentPatrolPoint;

    [SerializeField] float _speed;


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

    public FactionsEnum Faction { get => _faction; set => _faction = value; }
    public CapsuleCollider CapsuleCollider { get => _capsuleCollider; set => _capsuleCollider = value; }
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; set => _navMeshAgent = value; }
    public float AgentRadius { get => _agentRadius; set => _agentRadius = value; }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _currentWeaponIndex = GetWeaponIndex(_unitType);
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshFilter = GetComponentInChildren<MeshFilter>();
        _material = _meshRenderer.material;
        _customAnimator = GetComponent<CustomAnimator>();
        _pooledObject = GetComponent<PooledObject>();

        _agentRadius = _navMeshAgent.radius;
        _capsuleRadius = _capsuleCollider.radius;

    }

    private void Start()
    {
        _objectifID = Random.Range(0, GameManager.instance.Objectifs.Length);
    }
    private void OnEnable()
    {
        _navMeshAgent.speed = _speed;
        
    }


    private void Update()
    {
        if (_isAlive)
        {
            HandleChase();

            if (_meshRenderer.sharedMaterial.color != Color.white)
            {
                _material.color = Vector4.Lerp(_material.color, Color.white, Time.deltaTime * 3.0f);
            }
        }
        else
        {
            if (_meshRenderer.sharedMaterial.color != Color.black)
            {
                _material.color = Vector4.Lerp(_material.color, Color.black, Time.deltaTime * 3.0f);
            }


            transform.Translate(-transform.up * Time.deltaTime * 0.3f);
            currentTimeSinceDeath += Time.deltaTime;

            if (currentTimeSinceDeath >= 3.0f)
            {
                currentTimeSinceDeath = 0;
                AISpawner_Manager.instance.UnSpawn(_pooledObject);
            }

        }

    }

    #region Global AI Functions

    public void SetUpAI(int faction, int unit)
    {
        _faction = (FactionsEnum)faction;
        _unitType = (SoldiersEnum)unit;

        _unitData = _unitsDataBank.GetUnitData((FactionsEnum)faction, (SoldiersEnum)unit);

        if(_unitData != null)
        {
            _meshFilter.sharedMesh = _unitData.UnitMesh;
            _customAnimator.AnimationsBank = _unitData.AnimationsBank;
            _material.SetTexture("_CurrentAnimMap", _unitData.AnimationsBank.GetAnimation(AnimationsTypes.Idle));
            _material.SetTexture("_NextAnimMap", _unitData.AnimationsBank.GetAnimation(AnimationsTypes.Idle));
        }
        else
        {
            Debug.Log("Unit Data is Null !");
        }

    }



    public void InitializedAI()
    {
        Debug.Log("Initialized AI Started");
        _isAlive = true;
        
        InitializeStats();
        FreezPhysics();

        _navMeshAgent.radius = _navMeshAgent.radius * 0.7f;
        _capsuleCollider.radius = _capsuleCollider.radius * 0.7f;

        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;

        _material.color = Color.white;
        _capsuleCollider.enabled = true;

        _deathEffect.gameObject.SetActive(false);

        _customAnimator.ResetAnimator();
    }

    public void SetBackAgentRadius()
    {
        _navMeshAgent.radius = _agentRadius;
        _capsuleCollider.radius = _capsuleRadius;
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
        //Instantiate<GameObject>(_ai_Data.DeathObject, transform.position, Quaternion.identity);
        _deathEffect.gameObject.SetActive(true);
        _deathEffect.Play();

        _isAlive = false;

        _customAnimator.Play(AnimationsTypes.Die);
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


        if (Vector3.Distance(transform.position, GameManager.instance.Objectifs[_objectifID].transform.position) > _unitData.AttackRange && !_navMeshAgent.hasPath)
        {
            _navMeshAgent.SetDestination(GameManager.instance.Objectifs[_objectifID].transform.position);
        }
        
        if(Vector3.Distance(transform.position, GameManager.instance.Objectifs[_objectifID].transform.position) <= _unitData.AttackRange)
        {
            if (_customAnimator.CurrentAnimation == AnimationsTypes.Walk)
            {
                _customAnimator.Play(AnimationsTypes.Idle);
            }

            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();

            _attackTimer += Time.deltaTime;
            HandleAIAttack();
        }
        else if(!_navMeshAgent.hasPath)
        {
            _navMeshAgent.SetDestination(GameManager.instance.Objectifs[_objectifID].transform.position);
            _customAnimator.Play(AnimationsTypes.Walk);
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
            _customAnimator.StartTrigger(AnimationsTypes.Attack);
            switch (_unitType)
            {
                case SoldiersEnum.Infantry:
                        LarbinA_Attack();
                    break;


                case SoldiersEnum.Scout:
                    LarbinA_Attack();
                    break;

                case SoldiersEnum.Cavalry:
                    LarbinA_Attack();
                    break;

                case SoldiersEnum.MiniBoss:
                    LarbinA_Attack();
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
        if(_aiStatistics.Count != 0)
        {
            foreach(Statistics myStat in _aiStatistics)
            {
                foreach (Statistics statistics in _unitData.AiStatistics)
                {
                    if (myStat._statName == statistics._statName)
                    {
                        myStat._statCurrentValue = statistics._statCurrentValue;
                    }
                }
            }

        }
        else
        {
            foreach (Statistics statistics in _unitData.AiStatistics)
            {
                _aiStatistics.Add(new Statistics(statistics._statName, statistics._statCurrentValue, statistics._statMaxValue));
            }
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
                        _customAnimator.StartTrigger(AnimationsTypes.GetHit);
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

    public FactionsEnum GetFaction()
    {
        return _faction;
    }
    #endregion
}
