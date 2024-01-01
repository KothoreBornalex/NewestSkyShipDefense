using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static IObjects;
using static MapManager;

public class ObjectifClass : MonoBehaviour, IObjects
{
    public enum ObjectifEnum
    {
        Gouvernaille,
        Mat,
        Barrils
    }

    [Header("Base Object Variables")]
    [SerializeField] private ObjectStates ObjectState;
    [SerializeField] private ObjectifEnum currentObjectif;

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

        /*foreach(LightStruct lightStruct in MapManager.instance.Lights)
        {
            if(lightStruct.lightPlace == currentObjectif)
            {
                lightStruct.SetOnFlame(true);
            }
        }*/

        for(int i = 0; i < MapManager.instance.Lights.Length; i++)
        {
            if (MapManager.instance.Lights[i].lightPlace == currentObjectif)
            {
                MapManager.instance.Lights[i].onFlame = true;
            }
        }
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
