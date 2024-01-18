using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseOverlayManager : MonoBehaviour
{
    // Fields

    [Header("Parameters")]
    [SerializeField] private float _blueBackgroundAppearSpeed;
    [SerializeField] private float _blueBackgroundDisappearSpeed;

    [Header("Overlay objects")]
    [SerializeField] private Image _redBackground;
    [SerializeField] private Image _blueBackground;
    [SerializeField] private TextMeshProUGUI _loseText;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private GameObject _objectToActivate;

    private bool _isActivated;
    private Coroutine _appearCoroutine;

    // Methods
    private void CheckActivation()
    {
        if (!_isActivated && GameManager.instance != null)
        {
            if(GameManager.instance.IsDefeated)
            {
                _isActivated = true;
                StartActivation();
            }
        }

    }

    [Button]
    private void StartActivation()
    {
        if(_appearCoroutine != null)
        {
            StopCoroutine(_appearCoroutine);
            _appearCoroutine = null;
        }
        if (GameManager.instance != null)
        {
            _waveText.text = _waveText + GameManager.instance.CurrentRound.ToString();
        }

        _appearCoroutine = StartCoroutine(AppearEffectCoroutine());
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckActivation();
    }

    IEnumerator AppearEffectCoroutine()
    {
        _blueBackground.gameObject.SetActive(true);
        _loseText.gameObject.SetActive(true);
        
        Color tempColor = _blueBackground.color;
        Color tempColorText = _loseText.color;

        while (_blueBackground.color.a < 0.95f)
        {

            tempColor.a = Mathf.Lerp(tempColor.a, 1f, Time.deltaTime * _blueBackgroundAppearSpeed);
            tempColorText.a = Mathf.Lerp(tempColorText.a, 1f, Time.deltaTime * _blueBackgroundAppearSpeed);

            _blueBackground.color = tempColor;
            _loseText.color = tempColorText;

            yield return null;
        }

        tempColor.a = 1f;
        tempColorText.a = 1f;
        _blueBackground.color = tempColor;
        _loseText.color = tempColorText;

        _objectToActivate.SetActive(true);

        while (_blueBackground.color.a > 0.05f)
        {
            Color tempColor2 = _blueBackground.color;

            tempColor2.a = Mathf.Lerp(tempColor2.a, 0f, Time.deltaTime * _blueBackgroundDisappearSpeed);
            if(tempColor2.a < 0.7f)
            {
                tempColor2.a = Mathf.Lerp(tempColor2.a, 0f, Time.deltaTime * _blueBackgroundDisappearSpeed * 3f);
            }

            _blueBackground.color = tempColor2;

            yield return null;
        }

        _blueBackground.gameObject.SetActive(false);

        yield return null;
    }
}
