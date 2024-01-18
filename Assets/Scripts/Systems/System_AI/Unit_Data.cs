using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AI_Class;
using static IStatistics;

[CreateAssetMenu(fileName = "AI_Data", menuName = "ScriptableObjects/AI_Data", order = 1)]
public class Unit_Data : ScriptableObject
{
    [Header("Important Data")]
    [SerializeField] private SoldiersEnum _unitType;
    [Space(15)]

    [Header("Graphics Data")]
    [SerializeField] private Mesh _unitMesh;
    [Expandable] [SerializeField] private ScriptableObject_AnimationsBank _animationsBank;
    [SerializeField] private GameObject _deathObject;
    [Space(15)]

    [Header("Stats Data")]
    [SerializeField] private List<Statistics> _aiStatistics = new List<Statistics>();
    [SerializeField, Range(0, 15)] private float _attackRange;


    #region GETTERS & SETTERS
    public SoldiersEnum UnitType { get => _unitType; }



    public Mesh UnitMesh { get => _unitMesh; }
    public ScriptableObject_AnimationsBank AnimationsBank { get => _animationsBank; }



    public float AttackRange { get => _attackRange;}
    public List<Statistics> AiStatistics { get => _aiStatistics;}
    public GameObject DeathObject { get => _deathObject;}

    #endregion

}
