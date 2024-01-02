using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell2Behavior : AttackBehavior
{
    // Fields

    [SerializeField] private float _duration;

    [SerializeField] private GameObject[] _spellParticles;

    // Properties


    // Methods

    public void SetSpellLevel(int level)
    {
        _spellParticles[level - 1].SetActive(true);
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
