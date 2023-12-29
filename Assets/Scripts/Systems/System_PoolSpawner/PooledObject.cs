using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private Action<PooledObject> _deathAction;
    private AI_Class _aiClass;
    int _factionIndex;
    int _soldierIndex;

    public int FactionIndex { get => _factionIndex;}
    public int SoldierIndex { get => _soldierIndex;}
    public AI_Class AiClass { get => _aiClass;}

    private void Awake()
    {
        _aiClass = GetComponent<AI_Class>();
    }


    public void InitalizedAction(Action<PooledObject> deathAction, int factionIndex, int soldierIndex)
    {
        _deathAction = deathAction;
        _factionIndex = factionIndex;
        _soldierIndex = soldierIndex;
    }



    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (collision.gameObject.CompareTag("Ground"))
        {
            _deathAction(this);
        }
        */
    }
}
