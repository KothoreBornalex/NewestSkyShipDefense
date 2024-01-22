using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    public enum ButtonState
    {
        Lighted,
        Unlighted
    }
    [SerializeField] private ButtonState _state;
    [SerializeField, Range(0.0f, 4.0f)] private float _blendSpeed;

    [SerializeField, Range(-1.0f, 5.0f)] private float _lightedButtonIntensity;
    [SerializeField, Range(-1.0f, 5.0f)] private float _unLightedButtonIntensity;

    private Material _materialInstance;
    private Color _baseColor;
    private Color _lightedColor;
    private Color _unLightedColor;

    public ButtonState State { get => _state; set => _state = value; }

    private void Start()
    {
        _materialInstance = GetComponent<Image>().material;
        _baseColor = _materialInstance.GetColor("_EmissionColor");
        _lightedColor = _baseColor * _lightedButtonIntensity;
        _unLightedColor = _baseColor * _unLightedButtonIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        _lightedColor = _baseColor * _lightedButtonIntensity;
        _unLightedColor = _baseColor * _unLightedButtonIntensity;
        switch (_state)
        {
            case ButtonState.Lighted:
                if (_materialInstance.GetColor("_EmissionColor") != _lightedColor)
                {
                    _materialInstance.SetColor("_EmissionColor", Vector4.Lerp(_materialInstance.GetColor("_EmissionColor"), _lightedColor, Time.deltaTime * _blendSpeed));
                }
                break;

            case ButtonState.Unlighted:
                if(_materialInstance.GetColor("_EmissionColor") != _unLightedColor)
                {
                    _materialInstance.SetColor("_EmissionColor", Vector4.Lerp(_materialInstance.GetColor("_EmissionColor"), _unLightedColor, Time.deltaTime * _blendSpeed));
                }
                break;

        }
    }

    private void OnDestroy()
    {
        _materialInstance.SetColor("_EmissionColor", _baseColor);
    }

}
