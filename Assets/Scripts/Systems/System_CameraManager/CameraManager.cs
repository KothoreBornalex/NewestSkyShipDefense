using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera cameraComponent;
    public Transform cameraTransform;
    public float swayAmount = 1.0f;
    public float swaySpeed = 1.0f; 
    public Vector2 swayDirection = new Vector2(1.0f, 1.0f);
    public float rotationAmountX = 10.0f;
    public float rotationAmountY = 5.0f;
    public float rotationAmountZ = 0.0f;
    public float rotationSpeed = 2.0f;

    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    private float timeCounter = 0.0f;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        initialCameraPosition = cameraTransform.localPosition;
        initialCameraRotation = cameraTransform.localRotation;


    }

    void Update()
    {
        // Calculate the sway motion using Perlin noise
        float xOffset = Mathf.PerlinNoise(timeCounter, 0) * 2.0f - 1.0f;
        float yOffset = Mathf.PerlinNoise(0, timeCounter) * 2.0f - 1.0f;

        // Calculate the rotation using Perlin noise
        float xRotation = Mathf.PerlinNoise(timeCounter, 0) * 2.0f - 1.0f;
        float yRotation = Mathf.PerlinNoise(0, timeCounter) * 2.0f - 1.0f;
        float zRotation = Mathf.PerlinNoise(timeCounter * 2, timeCounter * 2) * 2.0f - 1.0f;

        Vector3 sway = new Vector3(xOffset, yOffset, 0) * swayAmount;
        Quaternion rotation = Quaternion.Euler(xRotation * (rotationAmountX * Mathf.Cos(Time.time)), yRotation * (rotationAmountY * Mathf.Cos(Time.time)), zRotation * (rotationAmountZ * Mathf.Cos(Time.time))) * initialCameraRotation;

        cameraTransform.localPosition = initialCameraPosition + sway;
        cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, rotation, Time.deltaTime * rotationSpeed);

        // Update the time counter for Perlin noise
        timeCounter += Time.deltaTime * swaySpeed;
    }
}
