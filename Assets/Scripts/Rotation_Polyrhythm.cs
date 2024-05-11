using System.Collections;
using UnityEngine;

public class Rotation_Polyrhythm : MonoBehaviour
{
    public Polyrhythm Polyrhythm;
    public SongTimerV2 SongTimerV2;
    public CameraMover CameraMover;
    public GameObject objectTemplate; // Single GameObject template to be duplicated
    private GameObject[] objects; // Array to hold the instantiated GameObjects
    private float[] rotationSpeeds; // Rotation speeds calculated from intervals
    private GameObject rotationCenter; // Central point for rotation
    private float[] currentAngles; // Current rotation angles for each object
    private float cameraCurrentAngle1 = 0; // Current rotation angle for the camera
    private bool isInitialized = false; // Track if initialization has been done

    // Cameras
    public GameObject cameraObject1; // Camera GameObject
    public float cameraRadius1 = 0f; // Radius to offset the camera from the center
    public float cameraSpeed1 = 0f; // Speed at which the camera rotates
    public Vector3 cameraPositionOffset1; // Rotation offset for the camera
    public Vector3 cameraRotationOffset1; // Rotation offset for the camera

    void Start()
    {
        rotationCenter = new GameObject("RotationCenter");
        rotationCenter.transform.position = Vector3.zero; // Set the central point
    }

    void Update()
    {
        if (!isInitialized)
        {
            if (Polyrhythm == null || Polyrhythm.polyrhythmIntervals == null || Polyrhythm.polyrhythmIntervals.Length == 0)
            {
                Debug.LogError("Polyrhythm script or polyrhythmIntervals array is not set.");
                return;
            }

            if (objectTemplate == null)
            {
                Debug.LogError("Object template is not assigned.");
                return;
            }

            InitializeObjects();
            isInitialized = true;
        }

        if (SongTimerV2.started) {
            for (int i = 0; i < objects.Length; i++)
            {
                // Increment current angle based on rotation speed
                currentAngles[i] += rotationSpeeds[i] * Time.deltaTime;

                // Calculate new position using circular motion logic
                float radius = 1.05f * i; // Set radius as function of index to spread out objects
                float x = radius * Mathf.Cos(currentAngles[i] * Mathf.Deg2Rad);
                float z = radius * Mathf.Sin(currentAngles[i] * Mathf.Deg2Rad);

                // Update the position of the object
                objects[i].transform.localPosition = new Vector3(x, CameraMover.cameraYPosition-CameraMover.cameraInitialYPosition, z);
            }

            // // Update camera rotation and position
            // if (objects.Length > 0) {
            //     float lastObjectSpeed = rotationSpeeds[objects.Length - 1]; // Speed of the last object
            //     cameraCurrentAngle1 += lastObjectSpeed * Time.deltaTime; // Rotate at the same speed as the last object

            //     float cameraX1 = cameraRadius1 * Mathf.Cos(cameraCurrentAngle1 * Mathf.Deg2Rad * cameraSpeed1);
            //     float cameraZ1 = cameraPositionOffset1.z + cameraRadius1 * Mathf.Sin(cameraCurrentAngle1 * Mathf.Deg2Rad * cameraSpeed1);
            //     cameraX1 += cameraPositionOffset1.x * Mathf.Cos(cameraCurrentAngle1 * Mathf.Deg2Rad * cameraSpeed1);
            //     cameraZ1 += cameraPositionOffset1.z * Mathf.Cos(cameraCurrentAngle1 * Mathf.Deg2Rad * cameraSpeed1);

            //     // Update the position of the camera
            //     cameraObject1.transform.position = new Vector3(cameraX1, cameraPositionOffset1.y, cameraZ1);
            //     cameraObject1.transform.LookAt(rotationCenter.transform); // Ensure the camera always points towards the center
            //     cameraObject1.transform.Rotate(cameraRotationOffset1); // Apply rotation offset
            // }
        }
    }

    void InitializeObjects()
    {
        int objectCount = Polyrhythm.polyrhythmIntervals.Length;
        objects = new GameObject[objectCount];
        rotationSpeeds = new float[objectCount];
        currentAngles = new float[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            objects[i] = Instantiate(objectTemplate, rotationCenter.transform);
            objects[i].name = $"Object_{i}";

            float offset = i * 1.05f;
            objects[i].transform.localPosition = new Vector3(offset, 0, 0);
            objects[i].transform.localScale = Vector3.one;

            rotationSpeeds[i] = 360f / (Polyrhythm.polyrhythmIntervals[objectCount - 1 - i] / 1000f);
            currentAngles[i] = 0;

            Debug.Log($"Object {i} has interval {Polyrhythm.polyrhythmIntervals[objectCount - 1 - i]}ms and rotation speed {rotationSpeeds[i]}");
        }
    }
}
