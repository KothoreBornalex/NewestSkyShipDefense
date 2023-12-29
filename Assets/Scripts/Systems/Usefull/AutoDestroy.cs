using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDestroyObject(_timer));
    }

    private IEnumerator AutoDestroyObject(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
