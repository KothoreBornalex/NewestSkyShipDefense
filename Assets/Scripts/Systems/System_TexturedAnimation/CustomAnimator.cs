using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableObject_AnimationsBank;

public class CustomAnimator : MonoBehaviour
{

    [Header("Base Fields")]
    [SerializeField] private ScriptableObject_AnimationsBank _animationsBank;
    [SerializeField] private AnimationsTypes _currentAnimation;
    [SerializeField] private AnimationsTypes _baseAnimation;
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _animatedMaterialInstance;

    [Header("Transition Settings")]
    [SerializeField,Range(0, 2.5f)] private float _maxTransitionTime;
    private float _currentTransitionTime;

    [Range(0, 1)] private float _currentBlendValue;
    private bool _isTransitionning;

    private bool _isTrigger;
    private float _triggeredAnimationTimer;

    [Header("Debug Parameter")]
    [SerializeField] private AnimationsTypes _targetedAnimations;
    [Button("Play Target Animation")] void StartPlayAnimation() => Play(_targetedAnimations);
    [Button("Trigger Target Animation")] void StartTriggerAnimation() => Trigger(_targetedAnimations);

    private void Awake()
    {
        _animatedMaterialInstance = _meshRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {

        if (_isTransitionning)
        {
            _currentTransitionTime += Time.deltaTime;
            float blendIncrementValue = (1 / _maxTransitionTime) * Time.deltaTime;
            _animatedMaterialInstance.SetFloat("_Blend", _currentBlendValue + blendIncrementValue);

            //To keep count of what's the current value of the blending without having to get it from the material.
            _currentBlendValue += blendIncrementValue;

            if (_currentBlendValue >= 1.0f)
            {
                ApplyAnimation();
                _currentBlendValue = 0;
            }

        }

        if (_isTrigger && !_isTransitionning)
        {
            _triggeredAnimationTimer += Time.deltaTime;

            if(_triggeredAnimationTimer >= _animationsBank.GetAnimationLength(_currentAnimation) - _maxTransitionTime)
            {
                _currentAnimation = _baseAnimation;
                Play(_currentAnimation);
                _isTrigger = false;
                _triggeredAnimationTimer = 0;
            }
        }

    }



    public void Trigger(AnimationsTypes animationType)
    {
        if (_baseAnimation != _currentAnimation && _currentAnimation != animationType)
        {
            _baseAnimation = _currentAnimation;
        }
        _currentAnimation = animationType;



        //Setting Transition Animation Values.
        _animatedMaterialInstance.SetTexture("_NextAnimMap", _animationsBank.GetAnimation(animationType));
        _animatedMaterialInstance.SetFloat("_NextAnimLen", _animationsBank.GetAnimationLength(animationType));
        SetIsTransitionning(true);
        _isTrigger = true;
    }

    public void Play(AnimationsTypes animationType)
    {
        _currentAnimation = animationType;
        _baseAnimation = animationType;

        //Setting Transition Animation Values.
        _animatedMaterialInstance.SetTexture("_NextAnimMap", _animationsBank.GetAnimation(animationType));
        _animatedMaterialInstance.SetFloat("_NextAnimLen", _animationsBank.GetAnimationLength(animationType));
        SetIsTransitionning(true);
    }


    private void ApplyAnimation()
    {
        //Set Current Animation Values
        _animatedMaterialInstance.SetTexture("_CurrentAnimMap", _animationsBank.GetAnimation(_currentAnimation));
        _animatedMaterialInstance.SetFloat("_CurrentAnimLen", _animationsBank.GetAnimationLength(_currentAnimation));

        //Reset blending
        _animatedMaterialInstance.SetFloat("_Blend", 0);

        SetIsTransitionning(false);
    }

    private void SetIsTransitionning(bool value)
    {
        if(value)
        {
            _isTransitionning = true;
        }
        else
        {
            _isTransitionning = false;
        }
    }
}
