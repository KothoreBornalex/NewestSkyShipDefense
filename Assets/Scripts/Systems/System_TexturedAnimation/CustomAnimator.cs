using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ScriptableObject_AnimationsBank;

public class CustomAnimator : MonoBehaviour
{

    [Header("Base Fields")]
    [SerializeField] private ScriptableObject_AnimationsBank _animationsBank;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private AnimationsTypes _currentAnimation;
    [SerializeField] private AnimationsTypes _baseAnimation;
    private Material _animatedMaterialInstance;
    public AnimationsTypes CurrentAnimation { get => _currentAnimation;}



    [Header("Transition Settings")]
    [SerializeField,Range(0, 2.5f)] private float _maxTransitionTime;
    private float _currentAnimationTime;
    private float _nextAnimationTime;

    [Range(0, 1)] private float _currentBlendValue;
    private bool _isTransitionning;

    private bool _isTrigger;
    private bool _canAnimate = true;

    [Space(10)]
    [Header("Debug Parameter")]
    [SerializeField, Range(0, 2.0f)] private float debugSlowingAnimation = 1;

    [Space(10)]
    [SerializeField] private AnimationsTypes _targetedAnimations;

    [Button("Play Target Animation")] void StartPlayAnimation() => Play(_targetedAnimations);
    [Button("Trigger Target Animation")] void StartTriggerAnimation() => StartTrigger(_targetedAnimations);
    [Button("Reset")] void StartResetAnimator() => ResetAnimator();

    private void Awake()
    {
        _animatedMaterialInstance = _meshRenderer.material;
    }

    public void ResetAnimator()
    {
        _canAnimate = true;
        _baseAnimation = AnimationsTypes.Idle;
        _currentAnimation = AnimationsTypes.Idle;

        ApplyAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        //Don't change the order functions !
        HandleAnimationsTimers();

        HandleBlendingTransition();

        HandleTrigger();

        SetAnimationsTimers();
    }


    private void HandleTrigger()
    {
        if (_isTrigger && !_isTransitionning)
        {

            if (_currentAnimationTime >= _animationsBank.GetAnimationLength(_currentAnimation))
            {

                if (_currentAnimation == AnimationsTypes.Die)
                {
                    _canAnimate = false;
                    _currentAnimationTime = 0;
                    _isTrigger = false;

                }
                else
                {
                    _currentAnimationTime = _nextAnimationTime;
                    _currentAnimation = _baseAnimation;
                    ApplyAnimation();

                    _isTrigger = false;
                }

            }

        }
    }

    private void HandleBlendingTransition()
    {
        if (_isTransitionning)
        {
            float blendIncrementValue = ((1 / _maxTransitionTime) * Time.deltaTime) * debugSlowingAnimation;
            _animatedMaterialInstance.SetFloat("_Blend", _currentBlendValue + blendIncrementValue);

            //To keep count of what's the current value of the blending without having to get it from the material.
            _currentBlendValue += blendIncrementValue;

            if (_currentBlendValue >= 1.0f)
            {
                Debug.Log("EndBlend");
                ApplyAnimation();
                _currentBlendValue = 0;
                _currentAnimationTime = _nextAnimationTime;
            }
        }
    }

    private void HandleAnimationsTimers()
    {
        if (_canAnimate)
        {
            _currentAnimationTime += Time.deltaTime * debugSlowingAnimation;

            _nextAnimationTime += Time.deltaTime * debugSlowingAnimation;
        }
    }

    private void SetAnimationsTimers()
    {
        if (_currentAnimationTime >= _animatedMaterialInstance.GetFloat("_CurrentAnimLen")/* && !_isTransitionning*/)
        {

            if (CurrentAnimation == AnimationsTypes.Die && !_isTransitionning)
            {
                _canAnimate = false;
            }

            _currentAnimationTime = 0.0f;
        }
        _animatedMaterialInstance.SetFloat("_CurrentAnimationTime", _currentAnimationTime / _animationsBank.GetAnimationLength(_currentAnimation));


        if (_nextAnimationTime >= _animatedMaterialInstance.GetFloat("_NextAnimLen")/* && !_isTransitionning*/)
        {

            _nextAnimationTime = 0.0f;
        }
        _animatedMaterialInstance.SetFloat("_NextAnimationTime", _nextAnimationTime / _animatedMaterialInstance.GetFloat("_NextAnimLen"));
    }

    public void StartTrigger(AnimationsTypes animationType)
    {
        if (CheckCurrentAnimIsDying()) return;
        CheckNextAnimation();

        if (_baseAnimation != _currentAnimation && _currentAnimation != animationType)
        {
            _baseAnimation = _currentAnimation;
        }
        _currentAnimation = animationType;
        //if(AnimationsTypes.Die == animationType) _baseAnimation = animationType;



        //Setting Transition Animation Values.
        _animatedMaterialInstance.SetTexture("_NextAnimMap", _animationsBank.GetAnimation(animationType));
        _animatedMaterialInstance.SetFloat("_NextAnimLen", _animationsBank.GetAnimationLength(animationType));
        SetIsTransitionning(true);
        _isTrigger = true;
    }



    public void Play(AnimationsTypes animationType)
    {
        if (CheckCurrentAnimIsDying()) return;
        CheckNextAnimation();

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

    private void CheckNextAnimation()
    {
        //if(_nextAnimationTime >= _animatedMaterialInstance.GetFloat("_NextAnimLen") * 0.7)

        _nextAnimationTime = 0;
    }


    private bool CheckCurrentAnimIsDying()
    {
        if (CurrentAnimation == AnimationsTypes.Die) return true;
        else return false;
    }

    
    private void SetIsTransitionning(bool value)
    {
        Debug.Log("Set IsTransitionning Value");
        _isTransitionning = value;
    }
}
