using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;

    public void SetNewMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", _effectsSlider.value);
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
