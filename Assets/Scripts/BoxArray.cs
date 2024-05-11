using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxArray : MonoBehaviour
{
    public Polyrhythm Polyrhythm;
    public SongTimerV2 SongTimerV2;
    public GameObject objectTemplate; // Single GameObject template to be duplicated
    private bool isInitialized = false; // Track if initialization has been done
    private GameObject rotationCenter; // Center of rotation for the arrayed objects

    public float radius = 40f; // Radius of the circle on which objects are placed
    public float timeScale = 0.075f; // Scale factor for time to spatial translation

    void Start()
    {
        rotationCenter = new GameObject("RotationCenter");
        rotationCenter.transform.position = Vector3.zero; // Set the central point
    }

    void Update()
    {
        if (!isInitialized)
        {
            if (Polyrhythm == null || Polyrhythm.playTimes == null || Polyrhythm.playTimes.Count == 0)
            {
                Debug.LogError("Polyrhythm script or playTimes array is not set.");
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
    }

    void InitializeObjects()
    {
        int numNotes = Polyrhythm.numNotes;
        float angleStep = 360f / numNotes; // Calculate the angle between each object in degrees

        // Instantiate objects around the circle, position based on play time
        for (int i = 0; i < Polyrhythm.playTimes.Count; i++)
        {
            List<float> noteTimes = Polyrhythm.playTimes[i];
            foreach (var time in noteTimes)
            {
                float angleInRadians = angleStep * i * Mathf.Deg2Rad; // Convert angle to radians
                Vector3 objectPosition = new Vector3(
                    Mathf.Cos(angleInRadians) * radius, // X coordinate
                    time * timeScale, // Y coordinate based on play time
                    Mathf.Sin(angleInRadians) * radius // Z coordinate
                );

                GameObject newObj = Instantiate(objectTemplate, objectPosition, Quaternion.identity, rotationCenter.transform);
                newObj.name = $"Note_{i}_Time_{time}";
                newObj.transform.localScale = Vector3.one; // Set scale if needed

                // Debug.Log($"Note {i} for time {time}ms placed at position {objectPosition}");
            }
        }
    }
}
