using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItSelf : MonoBehaviour
{
    public enum RotateAxis
    {
        X, Y, Z
    }

    [SerializeField] private RotateAxis _rotationAxis;
    [SerializeField] private float _rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        switch (_rotationAxis)
        {
            case RotateAxis.X:
                transform.Rotate(transform.right, _rotateSpeed * Time.deltaTime);
                break;

            case RotateAxis.Y:
                transform.Rotate(transform.up, _rotateSpeed * Time.deltaTime, Space.World);
                break;

            case RotateAxis.Z:
                transform.Rotate(transform.forward, _rotateSpeed * Time.deltaTime);
                break;
        }

        
    }
}
