using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float speed = 75.0f; // Speed at which the object will move
    bool started = false;
    float t;
    public float cameraYPosition;
    public float cameraInitialYPosition;

    void Start()
    {
        cameraInitialYPosition = transform.position.y;
    }

    void Update()
    {
        if (!started) {
            t += Time.deltaTime;
            if (t > 2)
            {
                started = true;
            }
        }
        else {
            cameraYPosition = transform.position.y + speed * Time.deltaTime;
            // Move the object upwards along the y-axis at a constant speed
            transform.position = new Vector3(transform.position.x,
                                                cameraYPosition,
                                                transform.position.z);
        }
    }
}
