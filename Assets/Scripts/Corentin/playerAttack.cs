using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static IStatistics;

public class playerAttack : MonoBehaviour
{
    // Fields

    [SerializeField] private LayerMask _ennemyLayerMask;

    [SerializeField] private GameObject _spell1Prefab;
    [SerializeField] private GameObject _spell2Prefab;
    [SerializeField] private GameObject _spell3Prefab;

    private int _spellIndex = 1;

    private int _spell1Level = 0;
    private int _spell2Level = 0;
    private int _spell3Level = 0;

    [SerializeField] private PlayerAttacksData _playerAttacksData;


    private float minPressedTime = 0.2f;
    private float currentPressedTime;

    // Properties
    public int Spell1Level { get => _spell1Level; set => _spell1Level = value; }
    public int Spell2Level { get => _spell2Level; set => _spell2Level = value; }
    public int Spell3Level { get => _spell3Level; set => _spell3Level = value; }


    // Methods
    public void ChangeAttackIndex(int index)
    {
        _spellIndex = index;
    }
    public void ChangeSpell1Level(int index)
    {
        if (index == _spell1Level + 1)
        {
            _spell1Level = index;
        }
    }
    public void ChangeSpell2Level(int index)
    {
        if (index == _spell2Level + 1)
        {
            _spell2Level = index;
        }
    }
    public void ChangeSpell3Level(int index)
    {
        if (index == _spell3Level + 1)
        {
            _spell3Level = index;
        }
    }

    private void Attack()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin, ray.direction, out hit, 300.0f))
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

    private void UseSpell1(Vector3 attackOrigin, int damageValue, float radius)     // Faible dégats de zone
    {
        GameObject att = Instantiate(_spell1Prefab, attackOrigin, Quaternion.identity);
        Collider[] hitCollider = Physics.OverlapSphere(att.transform.position, radius, _ennemyLayerMask);
        DamageSpell(hitCollider, damageValue);
    }
    private void UseSpell2(Vector3 attackOrigin, int damageValue, float radius)     // Fort dégats précis
    {
        GameObject att = Instantiate(_spell2Prefab, attackOrigin, Quaternion.identity);
        Collider[] hitCollider = Physics.OverlapSphere(att.transform.position, radius, _ennemyLayerMask);
        DamageSpell(hitCollider, damageValue);
    }
    private void UseSpell3(Vector3 attackOrigin, int slowValue, float radius)       // Freeze
    {
        GameObject att = Instantiate(_spell3Prefab, attackOrigin, Quaternion.identity);
        Collider[] hitCollider = Physics.OverlapSphere(att.transform.position, radius, _ennemyLayerMask);
        SlowSpell(hitCollider, slowValue);
    }

    private void DamageSpell(Collider[] colliders, int damageValue)
    {
        if (colliders.Length != 0) Debug.Log("Enemy Detected");

        foreach (Collider collider in colliders)
        {
            // !!!!! Retirer la vie des ennemis
            collider.GetComponent<IStatistics>().DecreaseStat(StatName.Health, damageValue);
        }
    }

    private void SlowSpell(Collider[] colliders, int slowValue)
    {
        foreach(Collider collider in colliders)
        {
            if (collider.CompareTag("Ennemy"))
            {
                // !!!!! Slow ennemies
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Stationary)
            {
                currentPressedTime += Time.deltaTime;
            }

        }
        else if(currentPressedTime != 0)
        {
            currentPressedTime = 0;
        }


        if (currentPressedTime >= minPressedTime)
        {
            Attack();
            currentPressedTime = 0;
        }


        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }*/
    }
}
