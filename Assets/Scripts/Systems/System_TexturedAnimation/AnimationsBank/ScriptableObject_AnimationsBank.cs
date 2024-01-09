using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        public Texture2D Texture { get => _texture; set => _texture = value; }
        public float AnimLength { get => _animLength; set => _animLength = value; }



        public TexturedAnimation()
        {

        }

        public TexturedAnimation(Texture2D texture, float animLength)
        {
            _texture = texture;
            _animLength = animLength;
        }
    }


    [System.Serializable]
    public enum AnimationsTypes
    {
        Idle,
        Attack,
        GetHit,
        Die,
        Walk
    }

    [System.Serializable]
    public class KeyValuePair
    {
        public AnimationsTypes key;
        public TexturedAnimation value;
    }


    //[SerializeField] private Dictionary<AnimationsTypes, TexturedAnimation> _animations = new Dictionary<AnimationsTypes, TexturedAnimation>();

    [SerializeField] private List<KeyValuePair> animationsList = new List<KeyValuePair>();



    [Header("Animations Management")]
    [SerializeField] private AnimationsTypes _selectedAnimation;
    [SerializeField] private TexturedAnimation _newAnimation;

    [Button("Add New Animation")] void StartAddElement() => AddElement(_selectedAnimation, _newAnimation);
    [Button("Delete This Animation")] void StartDeleteElement() => DeleteElement(_selectedAnimation);
    [Button("Delete All Animations")] void StartDeleteAll() => DeleteAll();


    private void DeleteElement(AnimationsTypes animationType)
    {
        KeyValuePair itemToRemove = animationsList.Find(item => item.key == animationType);
        if (itemToRemove != null)
        {
            animationsList.Remove(itemToRemove);
            Debug.Log("Animation Removed Successfully !");
        }
        else
        {
            Debug.LogError("No Element Found !");
        }
    }

    private void DeleteAll()
    {
        animationsList.Clear();
        Debug.Log("All Animations Removed Successfully !");
    }

    public void AddElement(AnimationsTypes animationType, TexturedAnimation texturedAnimation)
    {
        KeyValuePair existingItem = animationsList.Find(item => item.key == animationType);
        if (existingItem != null)
        {
            Debug.LogError("Already Contains This Animation !");
        }
        else
        {
            animationsList.Add(new KeyValuePair { key = animationType, value = texturedAnimation });
            Debug.Log("Animation Added Successfully !");
        }
    }

    public Texture2D GetAnimation(AnimationsTypes animation)
    {
        KeyValuePair item = animationsList.Find(item => item.key == animation);
        return item != null ? item.value.Texture : null;
    }

    public float GetAnimationLength(AnimationsTypes animation)
    {
        KeyValuePair item = animationsList.Find(item => item.key == animation);
        return item != null ? item.value.AnimLength : 0f;
    }

}