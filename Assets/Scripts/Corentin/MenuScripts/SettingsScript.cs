using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] UniversalRenderPipelineAsset _pipeline;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;
    [SerializeField] private Slider _resolutionSlider;


    private bool _canSaveValue;

    private float _resumeTimeScale;

    public void SetNewVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", _effectsSlider.value);
        PlayerPrefs.SetFloat("ResolutionURP", _resolutionSlider.value);
        _pipeline.renderScale = _resolutionSlider.value;
    }
    public void PauseGame()
    {
        _resumeTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = _resumeTimeScale;
    }
    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            Debug.Log("Set to : " + PlayerPrefs.GetFloat("MusicVolume"));
            _musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume"));
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            _effectsSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("EffectsVolume"));
        }
        if (PlayerPrefs.HasKey("ResolutionURP"))
        {
            _effectsSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("ResolutionURP"));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
