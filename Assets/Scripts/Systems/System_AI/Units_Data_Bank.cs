using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AI_Class;

[CreateAssetMenu(fileName = "AI_Data_Bank", menuName = "ScriptableObjects/AI/AI_Data_Bank", order = 1)]
public class Units_Data_Bank : ScriptableObject
{
    [System.Serializable]
    public struct FactionsData
    {
        public FactionsEnum faction;
        [Expandable] public Unit_Data[] factionUnits;
    }

    [SerializeField] private FactionsData[] _factionsArray;

    public FactionsData[] FactionsArray { get => _factionsArray;}


    public Unit_Data GetUnitData(FactionsEnum faction, SoldiersEnum unit)
    {
        foreach(FactionsData f in _factionsArray)
        {
            if(f.faction == faction)
            {
                foreach (Unit_Data data in f.factionUnits)
                {
                    if (data.UnitType == unit)
                    {
                        return data;
                    }
                    
                }
            }
        }

        return null;
    }
}
