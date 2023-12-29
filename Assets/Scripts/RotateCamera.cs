using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private float currentRotationSpeed = 0f;
    [SerializeField] private float rotationSpeed = 3.5f;
    [SerializeField] private float smoothness = 0.7f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Stationary:
                    currentRotationSpeed = 0.0f;
                    break;

                case TouchPhase.Began:
                    break;

                case TouchPhase.Moved:
                    // Adjust the rotation speed based on touch delta position
                    currentRotationSpeed += touch.deltaPosition.x * Time.deltaTime * rotationSpeed;
                    break;

                case TouchPhase.Ended:
                    break;
            }
        }

        // Gradually decrease rotation speed for smooth deceleration
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0f, Time.deltaTime * smoothness);

        // Rotate the object based on the rotation speed
        transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime);
    }
}
