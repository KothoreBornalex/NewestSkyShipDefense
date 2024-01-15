using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXpManager : MonoBehaviour
{
    // Fields
    [SerializeField] private int _xpPerDeaths;
    [SerializeField] private int _currentXp;

    private int _checkTotatDeathThisRound;
    private int _totatDeathThisRound;

    // Properties
    public int CurrentXp { get => _currentXp; set => _currentXp = value; }


    // Methods

    public bool SpendXp(int value)
    {
        if (CurrentXp - value >= 0)
        {
            DecreaseXp(value);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void DecreaseXp(int value)
    {
        CurrentXp -= value;
        if(CurrentXp < 0)
        {
            CurrentXp = 0;
        }
    }
    public void IncreaseXp(int value)
    {
        CurrentXp += value;
    }
    private void CheckXpChangeValue()
    {
        if(_checkTotatDeathThisRound < _totatDeathThisRound)
        {
            IncreaseXp(_xpPerDeaths * (_totatDeathThisRound - _checkTotatDeathThisRound));
            _checkTotatDeathThisRound = _totatDeathThisRound;
        }
        else if (_checkTotatDeathThisRound > _totatDeathThisRound)
        {
            _checkTotatDeathThisRound -= _totatDeathThisRound;
        }
    }

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            _totatDeathThisRound = GameManager.instance.UnitsDeadThisRound;
        }
        _checkTotatDeathThisRound += _totatDeathThisRound;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance != null)
        {
            if (_totatDeathThisRound != GameManager.instance.UnitsDeadThisRound)
            {
                _totatDeathThisRound = GameManager.instance.UnitsDeadThisRound;
                CheckXpChangeValue();
            }
        }
    }
}
