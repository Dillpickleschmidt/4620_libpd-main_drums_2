using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SongTimer : MonoBehaviour
{
    public int deltaTimeMs;
    public int timeFromLastFrame;
    [SerializeField] int bpm = 120;
    public int beatLengthMs;
    [SerializeField] int beatsPerMeasure = 4;
    public float measureCounter = 1;
    float beatRamp;
    float halfBeatRamp;
    float quarterBeatRamp;
    public int beatCounter = 1;
    public int halfBeatCounter = 1;
    public int quarterBeatCounter = 1;
    public List<float> sections;
    float[] sectionsAccumulatedMeasures;
    bool[] sectionsActive;
    public bool started = false;
    int startDelay = 2000;
    int deltaTimeMs_global = 0;

    void Start()
    {
        deltaTimeMs = -1;
        beatRamp = 0;
        timeFromLastFrame = 0;
        beatLengthMs = 60000 / bpm; // 500ms
        // Debug.Log(sections.Count);
        sectionsActive = new bool[sections.Count]; // true if the section is on
        float accumulatedMeasures = 0;
        sectionsAccumulatedMeasures = new float[sections.Count];
        for (int i = 0; i < sections.Count; i++) {
            accumulatedMeasures += sections[i];
            sectionsAccumulatedMeasures[i] = accumulatedMeasures;
            // Debug.Log(sectionsAccumulatedMeasures[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeMs_global += Mathf.RoundToInt(Time.deltaTime * 1000);
        if (!started && deltaTimeMs_global >= startDelay) {
            deltaTimeMs = 0;
            beatRamp = 0;
            timeFromLastFrame = 0;
            started = true;
        }
        else if (started) {
            deltaTimeMs += Mathf.RoundToInt(Time.deltaTime * 1000);
            timeFromLastFrame = Mathf.RoundToInt(Time.deltaTime * 1000);
            beatRamp += timeFromLastFrame;
            halfBeatRamp += timeFromLastFrame;
            quarterBeatRamp += timeFromLastFrame;

            // Beat counter
            if (beatRamp >= beatLengthMs) {
                beatRamp %= beatLengthMs;
                beatCounter++;
                measureCounter += 1f/beatsPerMeasure;
                Debug.Log($"Beat: {beatCounter}, Measure: {measureCounter}");
            }

            // Half beat counter
            if (halfBeatRamp >= beatLengthMs/2) {
                halfBeatRamp %= beatLengthMs/2;
                halfBeatCounter++;
                // Debug.Log($"HalfBeat: {halfBeatCounter}, Measure: {measureCounter}");
            }

            // Check if the beat counter has reached the number of beats per measure
            if (beatCounter >= beatsPerMeasure) {
                beatCounter = 0;
                halfBeatCounter = 0;
                quarterBeatCounter = 0;
            }

            // Section logic
            for (int i = 0; i < sections.Count; i++) {
                if ((i-1 < 0 || measureCounter >= sectionsAccumulatedMeasures[i-1]) // in case previous is out of bounds (first section)
                                    && measureCounter < sectionsAccumulatedMeasures[i]
                                    && !sectionsActive[i]) {
                    for (int j = 0; j < i; j++) { // deactivate all previous sections
                        sectionsActive[j] = false;
                    }
                    sectionsActive[i] = true;
                    Debug.Log($"Section {i} is active");
                }
            }
        }
    }
    public int getActiveSection()
    {
        for (int i = 0; i < sectionsActive.Length; i++)
        {
            if (sectionsActive[i])
            {
                return i; // Return the active section
            }
        }

        return -1;  // Return -1 if no true value is found
    }
}