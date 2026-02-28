using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] 
    [Range(50f, 250f)] 
    [Tooltip("Controls how fast the camera orbits around the arena focal point.")]
    private float rotationSpeed = 100f;

    void Update()
    {
        // Get horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");

        // Rotate the Focal Point around the Y-axis
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
    }
}