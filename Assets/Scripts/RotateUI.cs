using UnityEngine;

public class RotateUI : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
