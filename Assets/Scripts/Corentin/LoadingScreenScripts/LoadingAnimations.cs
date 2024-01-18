using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingAnimations : MonoBehaviour
{

    [Header("LoadingTextAnimation")]
    [SerializeField] private float _timeToLoop;
    [SerializeField] private TextMeshProUGUI _loadingText;
    private Coroutine _loadingTextCoroutine;


    [Header("LoadingIconAnimation")]
    [SerializeField] private float _loadingIconSpeed;
    [SerializeField] private Transform _loadingIcon;
    private Coroutine _loadingIconAnimationCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _loadingIconAnimationCoroutine = StartCoroutine(LoadingIconAnimationCoroutine());
        _loadingTextCoroutine = StartCoroutine(LoadingTextCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadingIconAnimationCoroutine()
    {
        while (true)
        {
            Vector3 tempVector = _loadingIcon.rotation.eulerAngles;

            tempVector.z -= Time.deltaTime * _loadingIconSpeed;

            _loadingIcon.rotation = Quaternion.Euler(tempVector);

            yield return null;
        }

        yield return null;
    }
    IEnumerator LoadingTextCoroutine()
    {
        while (true)
        {
            _loadingText.text = "Loading";
            yield return new WaitForSeconds(_timeToLoop / 4f);

            _loadingText.text = "Loading.";
            yield return new WaitForSeconds(_timeToLoop / 4f);

            _loadingText.text = "Loading..";
            yield return new WaitForSeconds(_timeToLoop / 4f);

            _loadingText.text = "Loading...";
            yield return new WaitForSeconds(_timeToLoop / 4f);
        }

        yield return null;
    }
}
