using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static IObjects;
public class ObjectifClass : MonoBehaviour, IObjects
{
    [Header("Base Object Variables")]
    [SerializeField] private ObjectStates ObjectState;

    [SerializeField] private GameObject _effectsParentObject;
    [SerializeField] private List<EffectEmission> Destroyed_effectEmissions = new List<EffectEmission>();
    [SerializeField] private List<EffectEmission> Destroyed_secondaryEffectEmissions = new List<EffectEmission>();

    public void Destroyed()
    {
        //Instantiating all the destroyed effects.
        /*foreach(EffectEmission effect in Destroyed_effectEmissions)
        {
            Instantiate<GameObject>(effect.Prefab_Effect, effect.SpawnPoint_Effect);
        }

        foreach (EffectEmission effect in Destroyed_secondaryEffectEmissions)
        {
            Instantiate<GameObject>(effect.Prefab_Effect, effect.SpawnPoint_Effect);
        }*/

        _effectsParentObject.SetActive(true);

    }



    public void SwitchState(ObjectStates newState)
    {
        if(newState == ObjectStates.Perfect)
        {
            ObjectState = ObjectStates.Perfect;
        }

        if (newState == ObjectStates.LittleDamaged)
        {
            ObjectState = ObjectStates.LittleDamaged;
        }

        if (newState == ObjectStates.HighDamaged)
        {
            ObjectState = ObjectStates.HighDamaged;
        }

        if (newState == ObjectStates.Destroyed && ObjectState != ObjectStates.Destroyed)
        {
            ObjectState = ObjectStates.Destroyed;
            Destroyed();
        }
    }

    public ObjectStates GetObjectState()
    {
        return ObjectState;
    }

}
