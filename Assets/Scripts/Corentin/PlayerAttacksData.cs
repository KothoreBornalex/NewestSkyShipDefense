using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerAttacksData : ScriptableObject
{
    // Fields

    [SerializeField] private int[] _spell1DamageStats;
    [SerializeField] private int[] _spell1RadiusStats;

    [SerializeField] private int[] _spell2DamageStats;
    [SerializeField] private int[] _spell2RadiusStats;

    [SerializeField] private int[] _spell3SlowStats;
    [SerializeField] private int[] _spell3RadiusStats;


    // Properties
    public int[] Spell1DamageStats { get => _spell1DamageStats; set => _spell1DamageStats = value; }
    public int[] Spell1RadiusStats { get => _spell1RadiusStats; set => _spell1RadiusStats = value; }
    public int[] Spell2DamageStats { get => _spell2DamageStats; set => _spell2DamageStats = value; }
    public int[] Spell2RadiusStats { get => _spell2RadiusStats; set => _spell2RadiusStats = value; }
    public int[] Spell3SlowStats { get => _spell3SlowStats; set => _spell3SlowStats = value; }
    public int[] Spell3RadiusStats { get => _spell3RadiusStats; set => _spell3RadiusStats = value; }
}
