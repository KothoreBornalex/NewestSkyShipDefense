using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static IStatistics;

public class playerAttack : MonoBehaviour
{
    // Fields


    [SerializeField] private LayerMask _ennemyLayerMask;

    [Header("Spells")]
    [SerializeField] private GameObject _spell1Prefab;
    [SerializeField] private GameObject _spell2Prefab;
    [SerializeField] private GameObject _spell3Prefab;

    private int _spellIndex = 1;

    [SerializeField] private int _spell1Level = 0;
    [SerializeField] private int _spell2Level = 0;
    [SerializeField] private int _spell3Level = 0;

    [SerializeField] private float _timeBetweenSpellSoundEffect;
    private Coroutine _spellSoundEffectCooldown;
    private bool _canPlaySoundEffect = true;

    private PlayerXpManager _playerXpManager;

    [SerializeField] private PlayerAttacksData _playerAttacksData;

    [Header("Elements")]
    [SerializeField] private int _elementIndex;

    [Header("Mana")]
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;
    [SerializeField] private float _manaIncreaseSpeed;
    [SerializeField] private float _manaCostCastSpell;
    [SerializeField] private float _manaMaxIncreaseValue;
    private Coroutine _manaIncreaseCoroutine;

    
    private float minPressedTime = 0.2f;
    private float currentPressedTime;

    // Properties
    public int Spell1Level { get => _spell1Level; set => _spell1Level = value; }
    public int Spell2Level { get => _spell2Level; set => _spell2Level = value; }
    public int Spell3Level { get => _spell3Level; set => _spell3Level = value; }

    public float CurrentMana { get => _currentMana; set => _currentMana = value; }
    public float MaxMana { get => _maxMana; set => _maxMana = value; }


    // Methods
    public void ChangeAttackIndex(int index)
    {
        _spellIndex = index;
        if(SoundManager.instance != null)
        {
            SoundManager.instance.PlayChangeSpellEffect();
        }
    }
    public void ChangeSpell1Level(int index)
    {
        if (index == _spell1Level + 1)
        {
            bool canUpgrade = false;

            if (index <= 3)
            {
                canUpgrade = _playerXpManager.SpendXp(10);
            }
            else if (index <= 7)
            {
                canUpgrade = _playerXpManager.SpendXp(15);
            }
            else if (index <= 11)
            {
                canUpgrade = _playerXpManager.SpendXp(20);
            }

            if(canUpgrade)
            {
                _spell1Level = index;
                if(SoundManager.instance != null)
                {
                    SoundManager.instance.PlayUpgradeSound();
                }
            }
        }
    }
    public void ChangeSpell2Level(int index)
    {
        if (index == _spell2Level + 1)
        {
            bool canUpgrade = false;

            if (index <= 3)
            {
                canUpgrade = _playerXpManager.SpendXp(10);
            }
            else if (index <= 7)
            {
                canUpgrade = _playerXpManager.SpendXp(15);
            }
            else if (index <= 11)
            {
                canUpgrade = _playerXpManager.SpendXp(20);
            }

            if (canUpgrade)
            {
                _spell2Level = index;
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayUpgradeSound();
                }
            }
        }
    }
    public void ChangeSpell3Level(int index)
    {
        if (index == _spell3Level + 1)
        {
            bool canUpgrade = false;

            if (index <= 3)
            {
                canUpgrade = _playerXpManager.SpendXp(10);
            }
            else if (index <= 7)
            {
                canUpgrade = _playerXpManager.SpendXp(15);
            }
            else if (index <= 11)
            {
                canUpgrade = _playerXpManager.SpendXp(20);
            }

            if (canUpgrade)
            {
                _spell3Level = index;
                IncreaseManaMax(_manaMaxIncreaseValue);
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayUpgradeSound();
                }
            }
        }
    }

    public void ChangeElementIndex(int index)
    {
        if(index >= 0 && index < 3)
        {
            _elementIndex = index;
            if(SoundManager.instance != null)
            {
                SoundManager.instance.PlayChangeElementEffect();
            }
        }
    }


    private void Repare()
    {
        if (GameManager.instance != null)
        {
            if (_currentMana >= _manaCostCastSpell && !GameManager.instance.IsDefeated)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hit, 500.0f))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

                    Vector3 go = hit.point;

                    if(hit.collider.gameObject.TryGetComponent<ObjectifStats>(out ObjectifStats objectifStats))
                    {
                        objectifStats.IncreaseStat(StatName.Health, Random.Range(5, 10));
                        SoundManager.instance.PlayReparationEffect();
                    }

                }
            }
        }
    }

    private void Attack()
    {
        if (GameManager.instance != null)
        {
            if (_currentMana >= _manaCostCastSpell)
            {
                if (!GameManager.instance.IsDefeated)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray.origin, ray.direction, out hit, 300.0f))
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);

                        Vector3 go = hit.point;

                        switch (_spellIndex)
                        {
                            case 1:
                                UseSpell1(go, _playerAttacksData.Spell1DamageStats[_spell1Level], _playerAttacksData.Spell1RadiusStats[_spell1Level]);

                                break;
                            case 2:
                                UseSpell2(go, _playerAttacksData.Spell2DamageStats[_spell2Level], _playerAttacksData.Spell2RadiusStats[_spell2Level]);
                                break;
                            case 3:
                                UseSpell3(go, _playerAttacksData.Spell3SlowStats[_spell3Level], _playerAttacksData.Spell3RadiusStats[_spell3Level]);
                                break;
                            default:
                                Debug.LogWarning("Erreur ! Aucune attaque reconnue !");
                                break;
                        }

                    }
                }
            }
            else
            {
                SoundManager.instance.PlayNoManaSound();
            }
        }
    }

    public void IncreaseManaMax(float increaseValue)
    {
        _maxMana += increaseValue;
        _currentMana += increaseValue;
        if(_currentMana > _maxMana)
        {
            _currentMana = _maxMana;
        }
    }
    private void DecreaseMana(float decreaseValue)
    {
        _currentMana -= decreaseValue;
        _currentMana = Mathf.Clamp(_currentMana, 0, _maxMana);
    }
    private void UseSpell1(Vector3 attackOrigin, int damageValue, float radius)     // Faible dégats de zone
    {
        if (SoundManager.instance != null)
        {
            int index = 0;
            if (_spell1Level <= 3)
            {
                 index = 0;
            }
            else if (_spell1Level <= 7)
            {
                 index = 1;
            }
            else if (_spell1Level <= 11)
            {
                index = 2;
            }
            SoundManager.instance.PlayPreciseAttackEffect(index);
        }

        DecreaseMana(_manaCostCastSpell);

        GameObject att = Instantiate(_spell1Prefab, attackOrigin, Quaternion.identity);

        Spell1Behavior sp1B = att.GetComponent<Spell1Behavior>();
        sp1B.SetSpellLevel(_spell1Level);
        sp1B.SetSpellElement(_elementIndex);

        Collider[] hitCollider = Physics.OverlapSphere(att.transform.position, radius, _ennemyLayerMask);
        DamageSpell(hitCollider, damageValue);
    }
    private void UseSpell2(Vector3 attackOrigin, int damageValue, float radius)     // Fort dégats précis
    {
        if (SoundManager.instance != null)
        {
            int index = 0;
            if (_spell2Level <= 3)
            {
                index = 0;
            }
            else if (_spell2Level <= 7)
            {
                index = 1;
            }
            else if (_spell2Level <= 11)
            {
                index = 2;
            }
            SoundManager.instance.PlayZoneAttackEffect(index);
        }

        DecreaseMana(_manaCostCastSpell);

        GameObject att = Instantiate(_spell2Prefab, attackOrigin, Quaternion.identity);

        Spell2Behavior sp2B = att.GetComponent<Spell2Behavior>();
        sp2B.SetSpellLevel(_spell2Level);
        sp2B.SetSpellElement(_elementIndex);

        Collider[] hitCollider = Physics.OverlapSphere(att.transform.position, radius, _ennemyLayerMask);
        DamageSpell(hitCollider, damageValue);
    }
    private void UseSpell3(Vector3 attackOrigin, int slowValue, float radius)       // Freeze
    {
        if (SoundManager.instance != null)
        {
            int index = 0;
            if (_spell3Level <= 3)
            {
                index = 0;
            }
            else if (_spell3Level <= 7)
            {
                index = 1;
            }
            else if (_spell3Level <= 11)
            {
                index = 2;
            }
            SoundManager.instance.PlaySlowEffect(index);
        }

        DecreaseMana(_manaCostCastSpell);

        GameObject att = Instantiate(_spell3Prefab, attackOrigin, Quaternion.identity);

        Spell3Behavior sp3B = att.GetComponent<Spell3Behavior>();
        sp3B.SetSpellLevel(_spell3Level);
        sp3B.SetSpellElement(_elementIndex);

        Collider[] hitCollider = Physics.OverlapSphere(att.transform.position, radius, _ennemyLayerMask);
        SlowSpell(hitCollider, slowValue);
    }

    private void DamageSpell(Collider[] colliders, int damageValue)
    {
        if (colliders.Length != 0)
        {
            Debug.Log("Enemy Detected");
            SoundManager.instance.PlayEnnemyHitSound();
        }

        foreach (Collider collider in colliders)
        {
            switch (collider.GetComponent<IStatistics>().GetFaction())
            {
                case AI_Class.FactionsEnum.Elf:
                    if (_elementIndex == 0)
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue * 2f);
                    }
                    else if (_elementIndex == 1)
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue / 2f);
                    }
                    else
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue);
                    }
                    break;
                case AI_Class.FactionsEnum.Orc:
                    if (_elementIndex == 0)
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue);
                    }
                    else if (_elementIndex == 1)
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue / 2f);
                    }
                    else
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue * 2f);
                    }
                    break;
                case AI_Class.FactionsEnum.Necromancer:
                    if (_elementIndex == 0)
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue / 2f);
                    }
                    else if (_elementIndex == 1)
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue * 2f);
                    }
                    else
                    {
                        collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue);
                    }
                    break;
                default:
                    collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue);
                    break;
            }
        }
    }

    private void SlowSpell(Collider[] colliders, int slowValue)
    {
        foreach(Collider collider in colliders)
        {
            StartCoroutine(FreezeCooldownCoroutine(collider.GetComponent<NavMeshAgent>()));
        }
    }

    private void Awake()
    {
        _playerXpManager = GetComponent<PlayerXpManager>();
        _currentMana = _maxMana;
    }
    void Start()
    {
        _manaIncreaseCoroutine = StartCoroutine(ManaIncreaseCoroutine());
    }

    void Update()
    {


#if UNITY_EDITOR

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (GameManager.instance.CurrentGameState == GameManager.GameState.InWave)
            {
                Attack();
            }
            else
            {
                Repare();
            }

            currentPressedTime = 0;
        }

#endif


        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Ended)
            {
                if (GameManager.instance.CurrentGameState == GameManager.GameState.InWave)
                {
                    Attack();
                }
                else
                {
                    Repare();
                }

                currentPressedTime = 0;
            }

            if (touch.phase == TouchPhase.Stationary)
            {
                currentPressedTime += Time.deltaTime;
                Debug.Log("pressed : " + currentPressedTime);
            }

        }
        else if(currentPressedTime != 0)
        {
            currentPressedTime = 0;
        }


    }

    IEnumerator ManaIncreaseCoroutine()
    {
        while (true)
        {
            if (_currentMana < _maxMana)
            {
                _currentMana += Time.deltaTime * _manaIncreaseSpeed;
                _currentMana = Mathf.Clamp(_currentMana, 0, _maxMana);
            }


            yield return null;
        }

        yield return null;
    }

    IEnumerator FreezeCooldownCoroutine(NavMeshAgent navMeshAgent)
    {
        float tempSpeed = navMeshAgent.speed;

        navMeshAgent.speed = 0f;

        yield return new WaitForSeconds(5.0f);

        if(navMeshAgent != null)
        {
            navMeshAgent.speed = tempSpeed;
        }

        yield return null;
    }

    IEnumerator SpellSoundEffectCooldownCoroutine()
    {
        yield return new WaitForSeconds(_timeBetweenSpellSoundEffect);

        _canPlaySoundEffect = true;

        yield return null;
    }
}
