using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePagesManager : MonoBehaviour
{
    // Fields

    [Header("UpgradesHandler")]
    [SerializeField] private RectTransform _upgradesHandler;

    [Header("PagesIndicators")]
    [SerializeField] private RectTransform[] _pagesIndicators;
    private Vector3 _targetIndicatorPos;

    [Header("PagesSlideManager")]
    [SerializeField] private int _indexPages;
    [SerializeField] private int _totalPages;

    [SerializeField] private float _speedPages;
    private Coroutine _slidePageCoroutine;

    [Header("TestButtons")]
    [SerializeField] private bool testRight;
    [SerializeField] private bool testLeft;



    // Properties


    // Methods

    public void NextPages()
    {
        if(_indexPages + 1 < _totalPages)
        {
            _indexPages++;
            SlidePages();
        }
    }
    public void PreviousPages()
    {
        if(_indexPages > 0)
        {
            _indexPages--;
            SlidePages();
        }
    }
    private void SlidePages()
    {
        _targetIndicatorPos = _pagesIndicators[_indexPages].localPosition;
        if (_slidePageCoroutine == null)
        {
            _slidePageCoroutine = StartCoroutine(SlidePagesCoroutine());
        }
    }

    private void Awake()
    {
        _indexPages = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (testRight)
        {
            testRight = false;
            NextPages();
        }
        if (testLeft)
        {
            testLeft = false;
            PreviousPages();
        }
    }

    IEnumerator SlidePagesCoroutine()
    {
        Debug.Log("start coroutine bouge upgrade !");

        float tempX = _upgradesHandler.position.x;

        while (Mathf.Abs(_targetIndicatorPos.x - _upgradesHandler.localPosition.x) > 0.2f)
        {
            
            Vector3 tempVector = _upgradesHandler.localPosition;
            Debug.Log("target : " + _targetIndicatorPos);
            Debug.Log("before : " + tempX);
            tempX = Mathf.Lerp(tempX, _targetIndicatorPos.x, Time.deltaTime * _speedPages);
            Debug.Log("after : " + tempX);

            tempVector.x = tempX;

            _upgradesHandler.localPosition = tempVector;
            
            yield return null;
        }

        Debug.Log("end coroutine");
        _slidePageCoroutine = null;

        yield return null;
    }
}
