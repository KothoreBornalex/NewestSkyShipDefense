using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePagesManager : MonoBehaviour
{
    // Fields

    [SerializeField] private RectTransform _upgrades;
    private float _upgradeLimit;

    [SerializeField] private int _indexPages;
    [SerializeField] private int _totalPages;

    [SerializeField] private float _speedPages;

    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _previousPageButton;

    [SerializeField] private bool testRight;
    [SerializeField] private bool testLeft;

    private Coroutine _slidePageCoroutine;


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
        if (_slidePageCoroutine != null)
        {
            _slidePageCoroutine = null;
        }
        _slidePageCoroutine = StartCoroutine(SlidePagesCoroutine());
    }

    private void Awake()
    {
        _indexPages = 0;
        _upgradeLimit = _upgrades.position.x;
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
        float xLimit = 0;

        switch (_indexPages)
        {
            case 0:
                xLimit = _upgradeLimit;
                break;
            case 1:
                xLimit = 0;
                break;
            case 2:
                xLimit = -_upgradeLimit;
                break;
        }

        Vector3 tempPos = _upgrades.localPosition;
        //tempPos.x += xLimit;

        while ((_upgrades.localPosition - GetComponent<RectTransform>().localPosition).magnitude > 0.07f)
        {
            tempPos.x = Mathf.Lerp(tempPos.x, xLimit, Time.deltaTime * _speedPages);

            _upgrades.localPosition = tempPos;

            yield return null;
        }

        yield return null;
    }
}
