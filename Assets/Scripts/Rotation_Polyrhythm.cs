using System.Collections;
using UnityEngine;

public class Rotation_Polyrhythm : MonoBehaviour
{
    public Polyrhythm Polyrhythm;
    public SongTimerV2 SongTimerV2;
    private GameObject[] objects;
    private float[] rotationSpeeds; // Rotation speeds calculated from intervals
    private GameObject rotationCenter; // Central point for rotation
    private float[] currentAngles; // Current rotation angles for each sphere
    private bool isInitialized = false; // Track if initialization has been done

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

            InitializeObjects();
            isInitialized = true;
        }
        if (SongTimerV2.started) {
            for (int i = 0; i < objects.Length; i++)
            {
                // Increment current angle based on rotation speed
                currentAngles[i] += rotationSpeeds[i] * Time.deltaTime;

                // Calculate new position using circular motion logic
                float radius = 1.05f * i; // Set radius as function of index to spread out spheres
                float x = radius * Mathf.Cos(currentAngles[i] * Mathf.Deg2Rad);
                float z = radius * Mathf.Sin(currentAngles[i] * Mathf.Deg2Rad);

                // Update the position of the sphere
                objects[i].transform.localPosition = new Vector3(x, 0, z);
            }
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
            objects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objects[i].transform.SetParent(rotationCenter.transform);

            // Position spheres in a line initially along the x-axis
            float offset = i * 1.05f; // Incremental offset for each sphere
            objects[i].transform.localPosition = new Vector3(offset, 0, 0);
            objects[i].transform.localScale = Vector3.one;

            // Calculate the rotation speed based on the interval from the opposite end of the array
            rotationSpeeds[i] = 360f / (Polyrhythm.polyrhythmIntervals[objectCount - 1 - i] / 1000f);

            // Initialize angles
            currentAngles[i] = 0;

            Debug.Log($"Sphere {i} has interval {Polyrhythm.polyrhythmIntervals[objectCount - 1 - i]}ms and rotation speed {rotationSpeeds[i]}");
        }
    }
}
