using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationCloud : MonoBehaviour
{

    [SerializeField] private float _cloudsSpeed;
    
    [SerializeField] private Transform[] _cloudsHandlers;
    [SerializeField] private Transform _endLimit;
    [SerializeField] private Transform _startLimit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var cloudHandler in _cloudsHandlers)
        {
            cloudHandler.position += new Vector3(0f, 0f, -1f) * Time.deltaTime * _cloudsSpeed;
            if(cloudHandler.position.z < _endLimit.position.z)
            {
                cloudHandler.position = new Vector3(cloudHandler.position.x, cloudHandler.position.y, _startLimit.position.z);
            }
        }
    }
}
