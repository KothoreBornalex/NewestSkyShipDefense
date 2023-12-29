using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_AI_SpawnList", menuName = "ScriptableObjects/Data_AI_SpawnList", order = 1)]
public class SO_AI_SpawnList : ScriptableObject
{


    [System.Serializable]
    public class FactionList
    {
        private List<string> StringValues { get { return new List<string>() { "Orc", "Elf", "Human", "Necromancer" }; } }

        [Dropdown("StringValues")]
        public string Faction;
        public List<GameObject> list = new List<GameObject>();
    }

    [Header("Spawns Data")]
    [SerializeField, Range(0, 100)] private int _maxSpawnCountPerFaction;
    [SerializeField, Range(0, 10)] private int _baseSpawnCount = 4;
    [SerializeField, Range(0, 1000)] private int _crashTestSpawnCount = 100;

    [Header("Factions List")]
    [SerializeField] private List<FactionList> _factionsList;

    public List<FactionList> FactionsList { get => _factionsList;}
    public int MaxSpawnCountPerFaction { get => _maxSpawnCountPerFaction;}
    public int BaseSpawnCount { get => _baseSpawnCount;}
    public int CrashTestSpawnCount { get => _crashTestSpawnCount;}












    /*
    [Header("Faction Orc")]
    [SerializeField] private List<GameObject> _factionOrc = new List<GameObject>();
    [Space(20)]

    [Header("Faction Elf")]
    [SerializeField] private List<GameObject> _factionElf = new List<GameObject>();
    [Space(20)]

    [Header("Faction Necromancer")]
    [SerializeField] private List<GameObject> _factionNecromancer = new List<GameObject>();
    [Space(20)]

    [Header("Faction Human")]
    [SerializeField] private List<GameObject> _factionHuman = new List<GameObject>();

    public List<GameObject> FactionOrc { get => _factionOrc;}
    public List<GameObject> FactionElf { get => _factionElf;}
    public List<GameObject> FactionNecromancer { get => _factionNecromancer;}
    public List<GameObject> FactionHuman { get => _factionHuman;}
    */
}
