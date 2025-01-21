using UnityEngine;

public class CamSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.1f;   // Maximum amount of sway
    public float swaySpeed = 1.0f;    // Speed of the sway

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the camera
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate sway offset using sine waves
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed) * swayAmount * 0.5f; // Slightly different movement for Y

        // Apply the sway to the camera's position
        transform.localPosition = initialPosition + new Vector3(swayX, swayY, 0);
    }
}
