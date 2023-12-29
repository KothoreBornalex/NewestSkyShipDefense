using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NewWaveIndicatorBehavior : MonoBehaviour
{
    // Fields

    [Header("UIManager")]
    [SerializeField] private UpgradeUIManager _upgradeUIManager;

    [Header("SkullImage")]
    [SerializeField] private Image _indicatorSkullImage;

    [Header("Appear")]
    private Coroutine _appearCoroutine;
    private RectTransform _originalRectTransform;
    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    [SerializeField] private float _appearSpeed;

    [Header("Disappear")]
    private Coroutine _disappearCoroutine;
    [SerializeField] private RectTransform _disappearTarget;
    [SerializeField] private float _disappearSpeed;

    [Header("DisappearParticles")]
    [SerializeField] private UnityEvent _onDisappear;

    [Header("TransitionTrail")]
    [SerializeField] private RectTransform _trailHandler;
    [SerializeField] private float _trailSpeed;
    private Coroutine _trailRotateCoroutine;

    [Header("Tests")]
    [SerializeField] private bool _testButton;

    // Properties


    // Methods

    public void StartAnimationNewWave()
    {
        if (_appearCoroutine == null)
        {
            _indicatorSkullImage.gameObject.SetActive(true);

            _appearCoroutine = StartCoroutine(ApearCoroutine());
        }
    }
    private void SwitchToDisappear()
    {
        if (_disappearCoroutine == null)
        {
            _disappearCoroutine = StartCoroutine(DisapearCoroutine());
        }
    }
    private void ResetNewWave()
    {
        Color colorTemp = _indicatorSkullImage.color;
        colorTemp.a = 0f;
        _indicatorSkullImage.color = colorTemp;
        _indicatorSkullImage.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        _indicatorSkullImage.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 2f);
        _indicatorSkullImage.gameObject.SetActive(false);
    }

    private void Awake()
    {
        _originalRectTransform = _indicatorSkullImage.GetComponent<RectTransform>();
        _originalPosition = _originalRectTransform.position;
        _originalScale = _originalRectTransform.localScale;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (_testButton)
        {
            _testButton = false;

            StartAnimationNewWave();
        }
    }

    IEnumerator ApearCoroutine()
    {
        _indicatorSkullImage.GetComponent<RectTransform>().localScale = _originalScale;
        _indicatorSkullImage.GetComponent<RectTransform>().position = _originalPosition;

        Color colorTemp = _indicatorSkullImage.color;

        while (_indicatorSkullImage.color.a < 0.9f)
        {
            colorTemp.a = Mathf.Lerp(colorTemp.a, 1f, Time.deltaTime * _appearSpeed);

            Debug.Log(colorTemp.a);

            _indicatorSkullImage.color = colorTemp;

            yield return null;
        }

        _trailRotateCoroutine = null;
        _trailRotateCoroutine = StartCoroutine(RotateTrailCoroutine());
        
        yield return new WaitForSeconds(2f);

        _appearCoroutine = null;

        SwitchToDisappear();

        yield return null;
    }
    IEnumerator DisapearCoroutine()
    {
        while ((_indicatorSkullImage.GetComponent<RectTransform>().localScale - new Vector3(2f, 2f, 2f)).magnitude > 0.2f || (_indicatorSkullImage.GetComponent<RectTransform>().localPosition - _disappearTarget.localPosition).magnitude > 0.2f)
        {
            Debug.Log("In disappear");

            if ((_indicatorSkullImage.GetComponent<RectTransform>().localScale - new Vector3(2f, 2f, 2f)).magnitude > 0.2f)
            {
                float tempFloat = _indicatorSkullImage.GetComponent<RectTransform>().localScale.x;

                tempFloat = Mathf.Lerp(tempFloat, 2f, Time.deltaTime * _disappearSpeed);

                _indicatorSkullImage.GetComponent<RectTransform>().localScale = new Vector3(tempFloat, tempFloat, tempFloat);
            }

            if ((_indicatorSkullImage.GetComponent<RectTransform>().localPosition - _disappearTarget.localPosition).magnitude > 0.2f)
            {
                Vector3 tempVector = _indicatorSkullImage.GetComponent<RectTransform>().localPosition;

                tempVector.x = Mathf.Lerp(tempVector.x, _disappearTarget.localPosition.x, Time.deltaTime * _disappearSpeed);
                tempVector.y = Mathf.Lerp(tempVector.y, _disappearTarget.localPosition.y, Time.deltaTime * _disappearSpeed);
                tempVector.z = Mathf.Lerp(tempVector.z, _disappearTarget.localPosition.z, Time.deltaTime * _disappearSpeed);

                _indicatorSkullImage.GetComponent<RectTransform>().localPosition = tempVector;
            }

            yield return null;
        }

        _onDisappear.Invoke();

        ResetNewWave();

        _disappearCoroutine = null;

        yield return null;
    }

    IEnumerator RotateTrailCoroutine()
    {
        Vector3 rotation = _trailHandler.rotation.eulerAngles;

        rotation.z = 0;

        _trailHandler.rotation = Quaternion.Euler(rotation);

        while (rotation.z < 360f)
        {
            rotation.z = Mathf.Lerp(rotation.z, 362f, Time.deltaTime * _trailSpeed);

            _trailHandler.rotation = Quaternion.Euler(rotation);

            yield return null;
        }

        yield return null;
    }
}
