using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFeedBack : MonoBehaviour
{
    [Header("Timer Parameters")]
    [SerializeField, Range(0, 50)] private float _timer;

    [Header("FeedBacks Set")]
    [SerializeField] private MMFeedbacks _feedBack;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer(_timer));
    }

    private IEnumerator Timer(float timer)
    {
        yield return new WaitForSeconds(timer);
        LaunchFeedBack();
    }

    private void LaunchFeedBack()
    {
        _feedBack.PlayFeedbacks();
    }
}
