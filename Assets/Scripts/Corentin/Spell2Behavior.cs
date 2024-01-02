using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell2Behavior : AttackBehavior
{
    // Fields

    [SerializeField] private float _duration;

    [SerializeField] private GameObject[] _spellPhase1LevelsParticles;
    [SerializeField] private GameObject[] _spellPhase2LevelsParticles;
    [SerializeField] private GameObject[] _spellPhase3LevelsParticles;

    private GameObject[] _currentPhaseSpells;

    // Properties


    // Methods



    public void SetSpellLevel(int level)
    {
        if(level >= 0 && level < 4)
        {
            _currentPhaseSpells = _spellPhase1LevelsParticles;
        }
        else if (level >= 4 && level < 8)
        {
            _currentPhaseSpells = _spellPhase2LevelsParticles;
        }
        else if (level >= 8 && level < 12)
        {
            _currentPhaseSpells = _spellPhase3LevelsParticles;
        }
    }
    public void SetSpellElement(int indexE)
    {
        _currentPhaseSpells[indexE].SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpellTimeCast());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SpellTimeCast()
    {
        yield return new WaitForSeconds(_duration);

        Destroy(gameObject);
        yield return null;
    }
}
