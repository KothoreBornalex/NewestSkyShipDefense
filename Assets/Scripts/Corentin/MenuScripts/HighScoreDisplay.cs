using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highScoreOne;
    [SerializeField] private TextMeshProUGUI _highScoreTwo;
    [SerializeField] private TextMeshProUGUI _highScoreThree;


    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("FirstHighscore"))
        {
            _highScoreOne.text = PlayerPrefs.GetInt("FirstHighscore").ToString();
        }
        if (PlayerPrefs.HasKey("SecondHighscore"))
        {
            _highScoreTwo.text = PlayerPrefs.GetInt("SecondHighscore").ToString();
        }
        if (PlayerPrefs.HasKey("ThirdHighscore"))
        {
            _highScoreThree.text = PlayerPrefs.GetInt("ThirdHighscore").ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
