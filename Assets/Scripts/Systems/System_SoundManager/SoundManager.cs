using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    static SoundManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            instance = this;
        }
        else
        {
            instance = this;
        }
    }

    [Header("Musics")]
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioClip _pauseMusic;
    [SerializeField] private AudioClip _fightMusic;
    [SerializeField] private AudioClip _transitionMusic;
    [Space(20)]

    [SerializeField] private Transform _camera;
    private float _musicVolume = 0.7f;
    private float _soundVolume = 0.7f;

    private void PlaySound(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, _camera.position, _soundVolume);
    }
}
