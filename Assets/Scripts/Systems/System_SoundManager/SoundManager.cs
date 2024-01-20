using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Utilise Player Prefs pour les settings.
    //Fait un test voir si la fonction play vraiment un son.
    public static SoundManager instance;

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

        if (PlayerPrefs.HasKey("MusicVolume") == false)
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.7f);
        }
        if (PlayerPrefs.HasKey("EffectsVolume") == false)
        {
            PlayerPrefs.SetFloat("EffectsVolume", 0.7f);
        }
    }

    [Header("Musics")]
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioClip _pauseMusic;
    [SerializeField] private AudioClip _fightMusic;
    [SerializeField] private AudioClip _transitionMusic;
    [Space(20)]

    [Header("Effects")]
    [SerializeField] private AudioClip _clickEffect;
    [SerializeField] private AudioClip _sliderEffect;

    [SerializeField] private AudioClip _ennemyHitEffect;
    [SerializeField] private AudioClip _objectiveHitEffect;
    [SerializeField] private AudioClip _objectiveDestroyedEffect;
    
    [SerializeField] private AudioClip _newWaveEffect;
    [SerializeField] private AudioClip _loseEffect;
    
    [SerializeField] private AudioClip _upgradeEffect;
    [SerializeField] private AudioClip _noManaEffect;

    [Space(20)]

    [SerializeField] private Transform _camera;
    private float _musicVolume = 0.7f;
    private float _effectsVolume = 0.7f;

    public void PlaySound(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, _camera.position, _effectsVolume);
    }

    #region Effects
    public void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(_clickEffect, _camera.position, _effectsVolume);
    }
    public void PlaySliderSound()
    {
        AudioSource.PlayClipAtPoint(_sliderEffect, _camera.position, _effectsVolume);
    }
    public void PlayEnnemyHitSound()
    {
        AudioSource.PlayClipAtPoint(_ennemyHitEffect, _camera.position, _effectsVolume);
    }
    public void PlayObjectiveHitSound()
    {
        AudioSource.PlayClipAtPoint(_objectiveHitEffect, _camera.position, _effectsVolume);
    }
    public void PlayDestroyedSound()
    {
        AudioSource.PlayClipAtPoint(_objectiveDestroyedEffect, _camera.position, _effectsVolume);
    }
    public void PlayNewWaveSound()
    {
        AudioSource.PlayClipAtPoint(_newWaveEffect, _camera.position, _effectsVolume);
    }
    public void PlayLoseSound()
    {
        AudioSource.PlayClipAtPoint(_loseEffect, _camera.position, _effectsVolume);
    }
    public void PlayUpgradeSound()
    {
        AudioSource.PlayClipAtPoint(_upgradeEffect, _camera.position, _effectsVolume);
    }
    public void PlayNoManaSound()
    {
        AudioSource.PlayClipAtPoint(_noManaEffect, _camera.position, _effectsVolume);
    }
    #endregion

    private void Update()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            if(PlayerPrefs.GetFloat("MusicVolume") != _musicVolume)
            {
                _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            }
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            if (PlayerPrefs.GetFloat("EffectsVolume") != _effectsVolume)
            {
                _effectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
            }
        }
    }
}
