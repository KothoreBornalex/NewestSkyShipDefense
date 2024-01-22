using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        _audioSource = GetComponent<AudioSource>();

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
    //[SerializeField] private AudioClip _pauseMusic;
    //[SerializeField] private AudioClip _fightMusic;
    //[SerializeField] private AudioClip _transitionMusic;
    [SerializeField] private AudioClip[] _inGameMusics;
    private int _inGameMusicIndex = 0;

    [Space(20)]

    [Header("Effects")]
    [SerializeField] private AudioClip _clickEffect;
    [SerializeField] private AudioClip _sliderEffect;
    [SerializeField] private AudioClip _openUpgradesEffect;

    [SerializeField] private AudioClip[] _ennemyHitEffect;
    [SerializeField] private AudioClip[] _objectiveHitEffects;
    [SerializeField] private AudioClip _objectiveDestroyedEffect;
    
    [SerializeField] private AudioClip _newWaveEffect;
    [SerializeField] private AudioClip _loseEffect;
    [SerializeField] private AudioClip _secondLoseEffect;
    
    [SerializeField] private AudioClip _upgradeEffect;
    [SerializeField] private AudioClip _noManaEffect;
    [SerializeField] private AudioClip _changeSpellEffect;
    [SerializeField] private AudioClip _changeElementEffect;

    [SerializeField] private AudioClip[] _preciseAttackEffects;
    [SerializeField] private AudioClip[] _zoneAttackEffects;
    [SerializeField] private AudioClip[] _slowEffects;

    [SerializeField] private AudioClip[] _reparationEffects;

    [Header("Adjustment effects")]
    [SerializeField] private float _preciseAttackEffectAdjustment;
    [SerializeField] private float _zoneAttackEffectAdjustment;
    [SerializeField] private float _slowEffectAdjustment;

    [SerializeField] private float _newWaveEffectAdjustment;

    [SerializeField] private float _objectiveHitEffectAdjustment;

    [Space(20)]

    [SerializeField] private Transform _camera;
    private AudioSource _audioSource;
    private Coroutine _cooldownNextMusicCoroutine;
    private float _musicVolume = 0.7f;
    private float _effectsVolume = 0.7f;
    private bool _hasLostModifier;

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
        AudioSource.PlayClipAtPoint(_ennemyHitEffect[Random.Range(0, _ennemyHitEffect.Length)], _camera.position, _effectsVolume);
    }
    public void PlayObjectiveHitSound()
    {
        AudioSource.PlayClipAtPoint(_objectiveHitEffects[Random.Range(0, _objectiveHitEffects.Length)], _camera.position, _effectsVolume * _objectiveHitEffectAdjustment);
    }
    public void PlayDestroyedSound()
    {
        AudioSource.PlayClipAtPoint(_objectiveDestroyedEffect, _camera.position, _effectsVolume);
    }
    public void PlayNewWaveSound()
    {
        AudioSource.PlayClipAtPoint(_newWaveEffect, _camera.position, _effectsVolume * _newWaveEffectAdjustment);
    }
    public void PlayLoseSound()
    {
        _hasLostModifier = true;
        AudioSource.PlayClipAtPoint(_loseEffect, _camera.position, _effectsVolume);
        AudioSource.PlayClipAtPoint(_secondLoseEffect, _camera.position, _effectsVolume);
    }
    public void PlayUpgradeSound()
    {
        AudioSource.PlayClipAtPoint(_upgradeEffect, _camera.position, _effectsVolume);
    }
    public void PlayNoManaSound()
    {
        AudioSource.PlayClipAtPoint(_noManaEffect, _camera.position, _effectsVolume);
    }
    public void PlayChangeSpellEffect()
    {
        AudioSource.PlayClipAtPoint(_changeSpellEffect, _camera.position, _effectsVolume);
    }
    public void PlayChangeElementEffect()
    {
        AudioSource.PlayClipAtPoint(_changeElementEffect, _camera.position, _effectsVolume);
    }
    public void PlayOpenUpgradesEffect()
    {
        AudioSource.PlayClipAtPoint(_openUpgradesEffect, _camera.position, _effectsVolume);
    }
    public void PlayReparationEffect()
    {
        AudioSource.PlayClipAtPoint(_reparationEffects[Random.Range(0, _reparationEffects.Length)], _camera.position, _effectsVolume);
    }
    public void PlayPreciseAttackEffect(int index)
    {
        AudioSource.PlayClipAtPoint(_preciseAttackEffects[index], _camera.position, _effectsVolume * _preciseAttackEffectAdjustment);
    }
    public void PlayZoneAttackEffect(int index)
    {
        AudioSource.PlayClipAtPoint(_zoneAttackEffects[index], _camera.position, _effectsVolume * _zoneAttackEffectAdjustment);
    }
    public void PlaySlowEffect(int index)
    {
        AudioSource.PlayClipAtPoint(_slowEffects[index], _camera.position, _effectsVolume * _slowEffectAdjustment);
    }
    #endregion


    private void CheckMusicStillPlaying()
    {
        if (!_audioSource.isPlaying)
        {
            if (SceneManager.GetActiveScene().name == "MainMenuScene")
            {
                _audioSource.clip = _mainMenuMusic;
                _cooldownNextMusicCoroutine = StartCoroutine(CooldownBeforeNextMusicCoroutine());
            }
            if (SceneManager.GetActiveScene().name == "Final GameScene")
            {
                if(Time.timeScale == 0f)
                {
                    Debug.Log("NextMusic will not play in pause mode");
                }
                _inGameMusicIndex++;
                if(_inGameMusicIndex >= _inGameMusics.Length)
                {
                    _inGameMusicIndex = 0;
                }
                _audioSource.clip = _inGameMusics[_inGameMusicIndex];
                _cooldownNextMusicCoroutine = StartCoroutine(CooldownBeforeNextMusicCoroutine());
            }
        }
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            _audioSource.clip = _mainMenuMusic;
            _audioSource.Play();
        }
        if (SceneManager.GetActiveScene().name == "Final GameScene")
        {
            _audioSource.clip = _inGameMusics[_inGameMusicIndex];
            _audioSource.Play();
        }
    }
    private void Update()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            if(PlayerPrefs.GetFloat("MusicVolume") != _musicVolume)
            {
                _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
                _audioSource.volume = _musicVolume;
            }
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            if (PlayerPrefs.GetFloat("EffectsVolume") != _effectsVolume)
            {
                _effectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
            }
        }

        if(_cooldownNextMusicCoroutine == null)
        {
            CheckMusicStillPlaying();
        }

        if (_hasLostModifier)
        {
            _audioSource.volume = _musicVolume / 4f;
        }
    }

    IEnumerator CooldownBeforeNextMusicCoroutine()
    {
        yield return new WaitForSeconds(3f);

        _audioSource.Play();

        Debug.Log("RestartMusic");

        _cooldownNextMusicCoroutine = null;

        yield return null;
    }
}
