using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    // Fields

    [Header("Upgrades smooth")]
    [SerializeField] private RectTransform _upgradeSkillsPanel;

    [SerializeField] private RectTransform _openPosition;
    [SerializeField] private RectTransform _closedPosition;

    [SerializeField] private RectTransform _upgradesOpenPosition;
    [SerializeField] private RectTransform _upgradesClosePosition;

    [SerializeField] private RectTransform _upgradesPack;

    private bool _isOpen;
    private Coroutine _openCoroutine;
    private Coroutine _closeCoroutine;

    [SerializeField] private float _slideSpeed;

    [SerializeField] private RectTransform _openCloseIcon;
    [SerializeField] private float _rotationSpeed;

    [Header("Upgrades")]
    [SerializeField] private Button[] _upgradeButtons;
    [SerializeField] private Color _upgradedColor;
    [SerializeField] private Color _toUpgradeColor;

    [SerializeField] private playerAttack _playerAttack;
    [SerializeField] Slider _upgradeSliderSp1;
    [SerializeField] Slider _upgradeSliderSp2;
    [SerializeField] Slider _upgradeSliderSp3;

    [Header("Mana")]
    [SerializeField] private Slider _manaSlider;
    private float _manaMaxValue;
    private float _manaCurrentValue;
    private float _manaCheckValue;
    private Coroutine _manaSlideCoroutine;
    [SerializeField] private float _manaSlideSpeed;

    [Header("Wave")]
    [SerializeField] private int _waveCurrentValue;
    private int _waveCheckValue;
    [SerializeField] private TextMeshProUGUI _waveText;

    [Header("WaveFireParticles")]
    [SerializeField] private ParticleSystem _fireParticles;

    [Header("XP")]
    [SerializeField] private int _xpCurrentValue;
    private int _xpCheckValue;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private PlayerXpManager _playerXpManager;

    [Header("Spells")]
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;
    [SerializeField] private Image _spell1BackgroundImage;
    [SerializeField] private Image _spell2BackgroundImage;
    [SerializeField] private Image _spell3BackgroundImage;
    [SerializeField] private RectTransform _spellIndicatorRectTrans;
    [SerializeField] private float _spellIndicatorSpeed;
    private Vector3 _targetSpellIndPos;
    private Coroutine _spellIndicatorSlideCoroutine;

    [Header("Elements")]
    [SerializeField] private Image _elementTopBackgroundImage;
    [SerializeField] private Image _elementMidBackgroundImage;
    [SerializeField] private Image _elementBotBackgroundImage;
    [SerializeField] private RectTransform _indicatorRectTrans;
    [SerializeField] private float _indicatorSpeed;
    [SerializeField] private RectTransform _posTopRectTrans;
    [SerializeField] private RectTransform _posMidRectTrans;
    [SerializeField] private RectTransform _posBotRectTrans;
    private Vector3 _targetIndPos;
    private Coroutine _indicatorSlideCoroutine;

    [Header("Skip button")]
    [SerializeField] private GameObject _skipSideButton;
    Coroutine _skipPostWaveGameStateCoroutine;
    [SerializeField] float _skipSpeed;

    // Properties



    // Methods
    public void SkipPostWaveGameState()
    {
        if (GameManager.instance != null)
        {
            if(GameManager.instance.CurrentGameState == GameManager.GameState.PostWave && _skipPostWaveGameStateCoroutine == null)
            {
                _skipSideButton.SetActive(false);
                _skipPostWaveGameStateCoroutine = StartCoroutine(SkipPostWaveGameStateCoroutine());
            }
        }
    }
    private void ChangeSkipButtonState()
    {
        if(GameManager.instance != null)
        {
            if (GameManager.instance.CurrentGameState == GameManager.GameState.PostWave && _skipPostWaveGameStateCoroutine == null)
            {
                _skipSideButton.SetActive(true);
            }
            else
            {
                _skipSideButton.SetActive(false);
            }
        }
    }

    public void ChangeSpellColorState(int index)
    {
        switch (index)
        {
            case 1: // spell 1
                _spell1BackgroundImage.color = _selectedColor;
                _spell2BackgroundImage.color = _unselectedColor;
                _spell3BackgroundImage.color = _unselectedColor;
                _targetSpellIndPos = _spell1BackgroundImage.GetComponent<RectTransform>().localPosition;
                break;
            case 2: // spell 2
                _spell1BackgroundImage.color = _unselectedColor;
                _spell2BackgroundImage.color = _selectedColor;
                _spell3BackgroundImage.color = _unselectedColor;
                _targetSpellIndPos = _spell2BackgroundImage.GetComponent<RectTransform>().localPosition;
                break;
            case 3: // spell 3
                _spell1BackgroundImage.color = _unselectedColor;
                _spell2BackgroundImage.color = _unselectedColor;
                _spell3BackgroundImage.color = _selectedColor;
                _targetSpellIndPos = _spell3BackgroundImage.GetComponent<RectTransform>().localPosition;
                break;
        }
        if (_spellIndicatorSlideCoroutine == null)
        {
            _spellIndicatorSlideCoroutine = StartCoroutine(SpellIndicatorSlideCoroutine());
        }
    }
    public void ChangeElementColorState(int index)
    {
        switch (index)
        {
            case 1: // top
                _elementTopBackgroundImage.color = _selectedColor;
                _elementMidBackgroundImage.color = _unselectedColor;
                _elementBotBackgroundImage.color = _unselectedColor;
                _targetIndPos = _posTopRectTrans.localPosition;
                break;
            case 2: // mid
                _elementTopBackgroundImage.color = _unselectedColor;
                _elementMidBackgroundImage.color = _selectedColor;
                _elementBotBackgroundImage.color = _unselectedColor;
                _targetIndPos = _posMidRectTrans.localPosition;
                break;
            case 3: // bot
                _elementTopBackgroundImage.color = _unselectedColor;
                _elementMidBackgroundImage.color = _unselectedColor;
                _elementBotBackgroundImage.color = _selectedColor;
                _targetIndPos = _posBotRectTrans.localPosition;
                break;
        }
        if (_indicatorSlideCoroutine == null)
        {
            _indicatorSlideCoroutine = StartCoroutine(IndicatorSlideCoroutine());
        }
    }

    public void UpgradeSkillsPanelSlide()   // Manage the position and slide of upgrades Panel
    {
        if (_isOpen)
        {
            _isOpen = false;
            _closeCoroutine = StartCoroutine(CloseUpgradePanel());
            if (_openCloseIcon != null)
            {
                StartCoroutine(RotateCloseIcon());  // Rotate icon to arrow point top
            }
            else
            {
                Debug.LogWarning("Forgot to drag the icon ui button close/open so it won't rotate !!!");
            }
        }
        else
        {
            _isOpen = true;
            _openCoroutine = StartCoroutine(OpenUpgradePanel());
            if (_openCloseIcon != null)
            {
                StartCoroutine(RotateOpenIcon());   // Rotate icon to arrow point down
            }
            else
            {
                Debug.LogWarning("Forgot to drag the icon ui button close/open so it won't rotate !!!");
            }
        }
    }
    private void CheckStateLevels()     // Check if upgrade buttons must be in color in state acquired or not
    {
        // Ancien code qui faisait afficher tout les boutons en clair sauf les lumineux. Si on veut remmettre �a ne pas oublier de retirer les sp1Up0 etc... dans la liste upgrades buttons !!!
        //if (_playerAttack.Spell1Level != 0)
        //{
        //    if (_upgradeButtons[_playerAttack.Spell1Level - 1].GetComponent<Image>().color != _upgradedColor)
        //    {
        //        _upgradeButtons[_playerAttack.Spell1Level - 1].GetComponent<Image>().color = _upgradedColor;
        //        _upgradeSliderSp1.value = (float)_playerAttack.Spell1Level / 3f;
        //    }
        //}

        //if(_playerAttack.Spell2Level != 0)
        //{
        //    if (_upgradeButtons[_playerAttack.Spell2Level + 2].GetComponent<Image>().color != _upgradedColor)
        //    {
        //        _upgradeButtons[_playerAttack.Spell2Level + 2].GetComponent<Image>().color = _upgradedColor;
        //        _upgradeSliderSp2.value = (float) _playerAttack.Spell2Level / 3f;
        //    }
        //}

        //if (_playerAttack.Spell3Level != 0)
        //{
        //    if (_upgradeButtons[_playerAttack.Spell3Level + 5].GetComponent<Image>().color != _upgradedColor)
        //    {
        //        _upgradeButtons[_playerAttack.Spell3Level + 5].GetComponent<Image>().color = _upgradedColor;
        //        _upgradeSliderSp3.value = (float)_playerAttack.Spell3Level / 3f;
        //    }
        //}


        if (_upgradeButtons[_playerAttack.Spell1Level].GetComponent<Image>().color != _upgradedColor)
        {
            _upgradeButtons[_playerAttack.Spell1Level].GetComponent<Image>().color = _upgradedColor;
            float gap1 = 0;
            if (_playerAttack.Spell1Level >= 4)
            {
                gap1++;
                if (_playerAttack.Spell1Level >= 8)
                {
                    gap1++;
                }
            }
            _upgradeSliderSp1.value = ((float)_playerAttack.Spell1Level + gap1) / 13f;
            if (_playerAttack.Spell1Level > 0)
            {
                _upgradeButtons[_playerAttack.Spell1Level - 1].GetComponent<Image>().color = _toUpgradeColor;
            }
        }

        if (_upgradeButtons[_playerAttack.Spell2Level + 12].GetComponent<Image>().color != _upgradedColor)
        {
            _upgradeButtons[_playerAttack.Spell2Level + 12].GetComponent<Image>().color = _upgradedColor;

            float gap2 = 0;
            if(_playerAttack.Spell2Level >= 4)
            {
                gap2++;
                if (_playerAttack.Spell2Level >= 8)
                {
                    gap2++;
                }
            }
            _upgradeSliderSp2.value = ((float)_playerAttack.Spell2Level + gap2) / 13f;
            if (_playerAttack.Spell2Level > 0)
            {
                _upgradeButtons[_playerAttack.Spell2Level + 11].GetComponent<Image>().color = _toUpgradeColor;
            }
        }

        if (_upgradeButtons[_playerAttack.Spell3Level + 24].GetComponent<Image>().color != _upgradedColor)
        {
            _upgradeButtons[_playerAttack.Spell3Level + 24].GetComponent<Image>().color = _upgradedColor;
            float gap3 = 0;
            if (_playerAttack.Spell3Level >= 4)
            {
                gap3++;
                if (_playerAttack.Spell3Level >= 8)
                {
                    gap3++;
                }
            }
            _upgradeSliderSp3.value = ((float)_playerAttack.Spell3Level + gap3) / 13f;
            if (_playerAttack.Spell3Level > 0)
            {
                _upgradeButtons[_playerAttack.Spell3Level + 23].GetComponent<Image>().color = _toUpgradeColor;
            }
        }
    }

    private void CheckManaValue()    // Check stamina value in gameManager(i suppose it will be in GM or in player)
    {
        if(_manaCurrentValue != _playerAttack.CurrentMana)
        {
            _manaCurrentValue = _playerAttack.CurrentMana;
            UpdateManaSlider();
        }
    }
    private void CheckMaxManaValue()
    {
        if(_manaMaxValue != _playerAttack.MaxMana)
        {
            _manaMaxValue = _playerAttack.MaxMana;
            UpdateMaxManaSlider();
        }
    }
    private void UpdateMaxManaSlider()
    {
        _manaSlider.maxValue = _manaMaxValue;
    }
    private void UpdateManaSlider()  // Update state of stamina slider with coroutine
    {
        if (_manaSlideCoroutine != null)
        {
            StopCoroutine(_manaSlideCoroutine);
        }
        _manaSlideCoroutine = StartCoroutine(ManaSlideCoroutine());
    }

    private void CheckXpValue()    // Check Xp value in gameManager(i suppose it will be in GM or in player)
    {
        if (_xpCurrentValue != _playerXpManager.CurrentXp)
        {
            _xpCurrentValue = _playerXpManager.CurrentXp;
            UpdateXpText();
        }
    }
    private void UpdateXpText()
    {
        _xpText.text = _xpCurrentValue.ToString();
    }
    private void CheckWaveValue()   // Check Wave value in gameManager(i suppose it will be in GM or in player)
    {
        if (GameManager.instance != null)
        {
            if (_waveCurrentValue != GameManager.instance.CurrentRound)
            {
                _waveCurrentValue = GameManager.instance.CurrentRound;
                UpdateWaveText();
            }
        }
    }
    private void UpdateWaveText()
    {
        _waveText.text = _waveCurrentValue.ToString();
    }

    private void UpdateFireParticles()
    {
        var emission = _fireParticles.emission;
        //emission.rateOverTime = 30f * GameManager.instance.getWaveValue() * 0.3f;
        emission.rateOverTime = _waveCurrentValue * 30f * 0.3f;
        emission.rateOverTime = Mathf.Clamp(emission.rateOverTime.constant, 30f, 200);

        var radius = _fireParticles.shape;
        //radius.radius = GameManager.instance.getWaveValue() / 20f;
        radius.radius = _waveCurrentValue / 20f;
        radius.radius = Mathf.Clamp(radius.radius, 0.01f, 0.24f);

        var speed = _fireParticles.main.startSpeed;
        speed = _waveCurrentValue * 0.2f;
        speed = Mathf.Clamp(speed.constant, 1f, 5f);
    }

    private void OnValidate()
    {
        if (_manaCheckValue != _manaCurrentValue)
        {
            _manaCurrentValue = Mathf.Clamp(_manaCurrentValue, 0, _manaMaxValue);

            _manaCheckValue = _manaCurrentValue;

            UpdateManaSlider();
        }
        if (_xpCheckValue != _xpCurrentValue)
        {
            if (_xpCurrentValue < 0)
            {
                _xpCurrentValue = 0;
            }
            _xpCheckValue = _xpCurrentValue;
            UpdateXpText();
        }
        if (_waveCheckValue != _waveCurrentValue)
        {
            if (_waveCurrentValue < 0)
            {
                _waveCurrentValue = 0;
            }
            _waveCheckValue = _waveCurrentValue;
            UpdateWaveText();

            UpdateFireParticles();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _manaMaxValue = _playerAttack.MaxMana;
        _manaCurrentValue = _playerAttack.CurrentMana;
        _manaCheckValue = _manaCurrentValue;
        _manaSlider.maxValue = _manaMaxValue;
        UpdateManaSlider();
        UpdateWaveText();
        UpdateXpText();
        ChangeSpellColorState(1);
        ChangeElementColorState(1);
    }

    // Update is called once per frame
    void Update()
    {
        CheckStateLevels();
        CheckMaxManaValue();
        CheckManaValue();
        CheckWaveValue();
        CheckXpValue();
        ChangeSkipButtonState();
    }

    IEnumerator OpenUpgradePanel()
    {
        Vector3 positionTemp = _upgradeSkillsPanel.localPosition;

        Vector3 upPositionTemp = _upgradesPack.localPosition;

        while (((_upgradeSkillsPanel.localPosition.y != _openPosition.localPosition.y) || (_upgradesPack.localPosition != _upgradesOpenPosition.localPosition)) && _isOpen)
        {
            if(_upgradeSkillsPanel.localPosition.y != _openPosition.localPosition.y)
            {
                positionTemp.y = Mathf.Lerp(_upgradeSkillsPanel.localPosition.y, _openPosition.localPosition.y, Time.deltaTime * _slideSpeed);
                if (Mathf.Abs(_openPosition.localPosition.y - _upgradeSkillsPanel.localPosition.y) <= 0.1f)
                {
                    positionTemp.y = _openPosition.localPosition.y;
                }
            }
            if (_upgradesPack.localPosition != _upgradesOpenPosition.localPosition)
            {
                upPositionTemp.y = Mathf.Lerp(_upgradesPack.localPosition.y, _upgradesOpenPosition.localPosition.y, Time.deltaTime * _slideSpeed * 4f);
                if (Mathf.Abs(_upgradesOpenPosition.localPosition.y - _upgradesPack.localPosition.y) <= 0.1f)
                {
                    upPositionTemp.y = _upgradesOpenPosition.localPosition.y;
                }
            }

            _upgradeSkillsPanel.localPosition = positionTemp;

            _upgradesPack.localPosition = upPositionTemp;

            yield return null;
        }

        _openCoroutine = null;

        yield return null;
    }
    IEnumerator CloseUpgradePanel()
    {
        Vector3 positionTemp = _upgradeSkillsPanel.localPosition;

        Vector3 upPositionTemp = _upgradesPack.localPosition;

        while (((_upgradeSkillsPanel.localPosition.y != _closedPosition.localPosition.y) || (_upgradesPack.localPosition != _upgradesClosePosition.localPosition)) && !_isOpen)
        {
            if (_upgradeSkillsPanel.localPosition.y != _closedPosition.localPosition.y)
            {
                positionTemp.y = Mathf.Lerp(_upgradeSkillsPanel.localPosition.y, _closedPosition.localPosition.y, Time.deltaTime * _slideSpeed);
                if(Mathf.Abs(_closedPosition.localPosition.y - _upgradeSkillsPanel.localPosition.y) <= 0.05f)
                {
                    positionTemp.y = _closedPosition.localPosition.y;
                }
            }
            if (_upgradesPack.localPosition != _upgradesClosePosition.localPosition)
            {
                upPositionTemp.y = Mathf.Lerp(_upgradesPack.localPosition.y, _upgradesClosePosition.localPosition.y, Time.deltaTime * _slideSpeed * 4f);
                if (Mathf.Abs(_upgradesClosePosition.localPosition.y - _upgradesPack.localPosition.y) <= 0.05f)
                {
                    upPositionTemp.y = _upgradesClosePosition.localPosition.y;
                }
            }

            _upgradeSkillsPanel.localPosition = positionTemp;

            _upgradesPack.localPosition = upPositionTemp;

            yield return null;
        }

        _closeCoroutine = null;

        yield return null;
    }
    IEnumerator RotateOpenIcon()
    {
        Vector3 rotationTemp = _openCloseIcon.localRotation.eulerAngles;

        Vector3 rotationOpened = new Vector3(0, 0, 0);  // fleche pointant vers bas

        while (_openCloseIcon.localRotation.eulerAngles.z != rotationOpened.z && _isOpen)
        {
            rotationTemp.z = Mathf.Lerp(_openCloseIcon.localRotation.eulerAngles.z, rotationOpened.z, Time.deltaTime * _rotationSpeed);

            _openCloseIcon.localRotation = Quaternion.Euler(rotationTemp);

            yield return null;
        }

        yield return null;
    }
    IEnumerator RotateCloseIcon()
    {
        Vector3 rotationTemp = _openCloseIcon.localRotation.eulerAngles;

        Vector3 rotationClosed = new Vector3(0, 0, 180);  // fleche pointant vers haut

        while (_openCloseIcon.localRotation.eulerAngles.z != rotationClosed.z && !_isOpen)
        {
            rotationTemp.z = Mathf.Lerp(_openCloseIcon.localRotation.eulerAngles.z, rotationClosed.z, Time.deltaTime * _rotationSpeed);

            _openCloseIcon.localRotation = Quaternion.Euler(rotationTemp);

            yield return null;
        }

        yield return null;
    }


    IEnumerator ManaSlideCoroutine() // Coroutine to smooth stamina change visual in slider
    {
        float targetValue = (float)_manaCurrentValue;
        while (_manaSlider.value != targetValue)
        {
            _manaSlider.value = Mathf.Lerp(_manaSlider.value, targetValue, Time.deltaTime * _manaSlideSpeed);

            yield return null;
        }

        yield return null;
    }
    IEnumerator IndicatorSlideCoroutine()   // element indicator slide
    {
        while ((_indicatorRectTrans.localPosition - _targetIndPos).magnitude > 0.2f || _openCoroutine != null || _closeCoroutine != null)
        {
            Vector3 tempVector = _indicatorRectTrans.localPosition;

            float currentY = _indicatorRectTrans.localPosition.y;

            currentY = Mathf.Lerp(currentY, _targetIndPos.y, Time.deltaTime * _indicatorSpeed);

            tempVector.y = currentY;

            _indicatorRectTrans.localPosition = tempVector;

            yield return null;
        }

        _indicatorSlideCoroutine = null;

        yield return null;
    }
    IEnumerator SpellIndicatorSlideCoroutine()  // Spell indicator slide
    {
        while ((_spellIndicatorRectTrans.localPosition - _targetSpellIndPos).magnitude > 0.2f)
        {
            Vector3 tempVector = _spellIndicatorRectTrans.localPosition;

            float currentX = _spellIndicatorRectTrans.localPosition.x;

            currentX = Mathf.Lerp(currentX, _targetSpellIndPos.x, Time.deltaTime * _spellIndicatorSpeed);

            tempVector.x = currentX;

            _spellIndicatorRectTrans.localPosition = tempVector;

            yield return null;
        }

        _spellIndicatorSlideCoroutine = null;

        yield return null;
    }

    IEnumerator SkipPostWaveGameStateCoroutine()
    {
        float tempTimeScale = Time.timeScale;

        MapManager.instance.ShipMovingSpeed /= 5;
        MapManager.instance.ShipRotatingSpeed /= 5;

        Time.timeScale = _skipSpeed;

        while(GameManager.instance.CurrentGameState == GameManager.GameState.PostWave)
        {
            yield return null;
        }

        MapManager.instance.ShipMovingSpeed *= 5;
        MapManager.instance.ShipRotatingSpeed *= 5;

        Time.timeScale = tempTimeScale;

        _skipPostWaveGameStateCoroutine = null;
        StopCoroutine( _skipPostWaveGameStateCoroutine );

        yield return null;
    }
}
