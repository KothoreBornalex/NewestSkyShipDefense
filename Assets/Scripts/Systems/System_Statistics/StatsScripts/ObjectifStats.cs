using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IStatistics;
using static IObjects;
using NaughtyAttributes;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ObjectifStats : MonoBehaviour, IStatistics
{


    [Button("Take Damage")] void TakeDamage() => DecreaseStat(StatName.Health, Random.Range(5, 15));
    [Button("Reset")] void LaunchReset() => Reset();


    [Header("Objectif UI")]
    [SerializeField] private Color _hitColor;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _lifeSlider;
    [SerializeField] private Image _sliderBackGround;

    [Header("Objectif Stats")]
    private IObjects objectScript;
    [SerializeField] private Statistics _objectifHealth;

    public Statistics ObjectifHealth { get => _objectifHealth;}
    public IObjects ObjectScript { get => objectScript;}

    private void Reset()
    {
        objectScript = GetComponent<IObjects>();

        _objectifHealth = new Statistics();
        _objectifHealth._statName = StatName.Health;
        _objectifHealth._statMaxValue = 100;
        _objectifHealth._statCurrentValue = 100;
    }

    private void Start()
    {
        objectScript = GetComponent<IObjects>();
    }

    private void Update()
    {
        if(_lifeSlider != null && _sliderBackGround != null)
        {
            _lifeSlider.value = Mathf.Lerp(_lifeSlider.value, _objectifHealth._statCurrentValue, Time.deltaTime * 9.0f);
            _sliderBackGround.color = Vector4.Lerp(_sliderBackGround.color, Color.white, Time.deltaTime * 6.0f);
        }

        if(_objectifHealth._statCurrentValue == 0 && _canvasGroup.alpha > 0.16f)
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0.15f, Time.deltaTime * 0.5f);
        }
    }

    public void InitializeStats()
    {
        //Since the object only have one stat (Health) I don't initialize anything.
    }


    public void SetStat(StatName statName, float statValue)
    {
        if(statName == _objectifHealth._statName)
        {
            _objectifHealth._statCurrentValue = statValue;
        }
    }

    public void DecreaseStat(StatName statName, float decreasingValue)
    {
        //Since I only have one stat in this script, I don't need to do a for each to find the right stat.

        if (statName == _objectifHealth._statName)
        {
            _objectifHealth._statCurrentValue -= decreasingValue;
            _objectifHealth._statCurrentValue = Mathf.Clamp(_objectifHealth._statCurrentValue, 0, _objectifHealth._statMaxValue);

            //Changing Slider Color:
            if (_sliderBackGround != null)
            {
                _sliderBackGround.color = _hitColor;
            }

            // For Actualizing the object state.
            if (_objectifHealth._statCurrentValue <= 85 && _objectifHealth._statCurrentValue >= 50)
            {
                objectScript.SwitchState(ObjectStates.LittleDamaged);
            }

            if (_objectifHealth._statCurrentValue <= 50 && _objectifHealth._statCurrentValue >= 1)
            {
                objectScript.SwitchState(ObjectStates.HighDamaged);
            }

            if (_objectifHealth._statCurrentValue < 1)
            {
                objectScript.SwitchState(ObjectStates.Destroyed);
            }
        }


    }

    public void IncreaseStat(StatName statName, float increasingValue)
    {
        //Since I only have one stat in this script, I don't need to do a for each to find the right stat.

        if (statName == _objectifHealth._statName)
        {
            _objectifHealth._statCurrentValue += increasingValue;
            _objectifHealth._statCurrentValue = Mathf.Clamp(_objectifHealth._statCurrentValue, 0, _objectifHealth._statMaxValue);


            if (_objectifHealth._statCurrentValue >= 85)
            {
                objectScript.SwitchState(ObjectStates.Perfect);
            }

            if (_objectifHealth._statCurrentValue <= 85 && _objectifHealth._statCurrentValue >= 50)
            {
                objectScript.SwitchState(ObjectStates.LittleDamaged);
            }

            if (_objectifHealth._statCurrentValue <= 50 && _objectifHealth._statCurrentValue >= 1)
            {
                objectScript.SwitchState(ObjectStates.HighDamaged);
            }
        }
        
    }

}
