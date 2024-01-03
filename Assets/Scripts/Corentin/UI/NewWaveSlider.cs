using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewWaveSlider : MonoBehaviour
{

    [SerializeField] private Image _innerBorder;
    [SerializeField] private Image _outerBorder;
    [SerializeField] private Image _fill;


    public void SetNewWaveSliderAlpha(float alpha)
    {
        Color tempColor = _innerBorder.color;
        tempColor.a = alpha;
        _innerBorder.color = tempColor;

        tempColor = _outerBorder.color;
        tempColor.a = alpha;
        _outerBorder.color = tempColor;

        tempColor= _fill.color;
        tempColor.a = alpha;
        _fill.color = tempColor;
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
