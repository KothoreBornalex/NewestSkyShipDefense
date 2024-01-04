using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXpManager : MonoBehaviour
{
    // Fields

    [SerializeField] private int _currentXp;


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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
