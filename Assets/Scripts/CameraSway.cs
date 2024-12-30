using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float swayAmount = 0.05f;  // Amount of sway
    public float smoothness = 2f;    // Smoothness of sway
    public float maxSwayAngle = 2f; // Limit for sway angle

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Simulate sway based on mouse movement (adjust sensitivity for subtle effect)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate target rotation
        float swayX = Mathf.Clamp(-mouseY * swayAmount, -maxSwayAngle, maxSwayAngle);
        float swayY = Mathf.Clamp(mouseX * swayAmount, -maxSwayAngle, maxSwayAngle);
        Quaternion targetRotation = Quaternion.Euler(swayX, swayY, 0);

        // Smoothly interpolate between current and target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation * targetRotation, Time.deltaTime * smoothness);

        // Optional: Restore position if needed
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * smoothness);
    }
}