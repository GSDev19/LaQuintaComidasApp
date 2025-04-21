using UnityEngine;

public class RotateIcon : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation in degrees per second
    private void Update()
    {
        RotateInZAxes();
    }
    private void RotateInZAxes()
    {
        // Rotate the icon around its Z-axis
        transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
    }
}
