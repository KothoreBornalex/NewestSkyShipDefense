using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IStatistics;

[CreateAssetMenu(fileName = "TexturedAnimations", menuName = "ScriptableObjects/AnimationsBank", order = 1)]
public class ScriptableObject_AnimationsBank : ScriptableObject
{
    [System.Serializable]
    public class TexturedAnimation
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private float _animLength;
        [SerializeField] private GraphicsBuffer _firstFrame;

        public Texture2D Texture { get => _texture; set => _texture = value; }
        public float AnimLength { get => _animLength; set => _animLength = value; }
        public GraphicsBuffer FirstFrame { get => _firstFrame; set => _firstFrame = value; }



        public TexturedAnimation()
        {

        }

        public TexturedAnimation(Texture2D texture, float animLength, GraphicsBuffer firstFrame)
        {
            _texture = texture;
            _animLength = animLength;
            _firstFrame = firstFrame;
        }
    }


    public enum AnimationsTypes
    {
        Idle,
        Attack,
        Die,
        Walk
    }


    [SerializeField] private Dictionary<AnimationsTypes, TexturedAnimation> _animations = new Dictionary<AnimationsTypes, TexturedAnimation>();


    [Header("Animations Management")]
    [SerializeField] private AnimationsTypes _selectedAnimation;
    [Button("Delete Animations")] void StartDeleteElement() => DeleteElement();
    [Button("Delete All Animations")] void StartDeleteAll() => DeleteAll();


    private void DeleteElement()
    {
        if(_animations.ContainsKey(_selectedAnimation))
        {
            _animations.Remove(_selectedAnimation);
        }
        else
        {
            Debug.LogError("No Element Found !");
        }
    }

    private void DeleteAll()
    {
        _animations.Clear();
    }

    public void AddElement(AnimationsTypes animationType, TexturedAnimation texturedAnimation)
    {
        if (_animations.ContainsKey(animationType))
        {
            Debug.LogError("Already Contains This Animation !");
        }
        else
        {
            _animations.Add(animationType, texturedAnimation);
        }
    }




    public Texture2D GetAnimation(AnimationsTypes animation)
    {
        return _animations[animation].Texture;
    }

    public float GetAnimationLength(AnimationsTypes animation)
    {
        return _animations[animation].AnimLength;
    }

    public GraphicsBuffer GetFirstFrame(AnimationsTypes animation)
    {
        return _animations[animation].FirstFrame;
    }
}