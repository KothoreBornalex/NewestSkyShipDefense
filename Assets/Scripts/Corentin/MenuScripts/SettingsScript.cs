using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;

    private float _resumeTimeScale;

    public void SetNewVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", _effectsSlider.value);
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
            _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            _musicSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
