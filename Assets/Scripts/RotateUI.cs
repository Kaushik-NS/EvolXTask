using UnityEngine;

// This script rotates a UI element continuously
// It is used on the image in Main menu screen

public class RotateUI : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float speed = 50f;

    void Update()
    {
        // Rotate the object every frame
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
