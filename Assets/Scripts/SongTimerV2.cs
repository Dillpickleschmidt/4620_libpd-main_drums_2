using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTimerV2 : MonoBehaviour
{
    public float t;
    int t_orig;
    int startDelay = 2000; // 2 seconds
    public float timeFromLastFrame;
    public bool started = false;
    int bpm = 120;
    int beatLengthMs;
    public List<float> sections;
    float[] sectionsAccumulated;
    float[] sectionStartTimes;
    float[] sectionEndTimes;
    public float measureRampMs;
    public int measureLengthMs;

    void Start()
    {
        t = -1;
        timeFromLastFrame = 0;
        beatLengthMs = 60000 / bpm; // 500ms
        measureLengthMs = beatLengthMs * 4; // 4 beats per measure

        // Add up the measures of each section
        float accumulatedMeasures = 0;
        sectionsAccumulated = new float[sections.Count];
        for (int i = 0; i < sections.Count; i++) {
            accumulatedMeasures += sections[i];
            sectionsAccumulated[i] = accumulatedMeasures;
            // Debug.Log(sectionsAccumulated[i]);
        }

        // Get section start and end times
        sectionStartTimes = new float[sections.Count];
        sectionEndTimes = new float[sections.Count];
        for (int i = 0; i < sections.Count; i++) {
            sectionStartTimes[i] = (i > 0 ? sectionsAccumulated[i-1] : 0f) * measureLengthMs;
            sectionEndTimes[i] = sectionsAccumulated[i] * measureLengthMs;
        }
    }

    // Update is called once per frame
    void Update()
    {
        t_orig += Mathf.RoundToInt(Time.deltaTime * 1000);
        if (!started && t_orig >= startDelay) {
            t = 0;
            timeFromLastFrame = 0;
            started = true;
        }
        else if (started) {
            timeFromLastFrame = Time.deltaTime * 1000; // Update time from last frame
            t += timeFromLastFrame; // Update time to the exact current time

            // Measure ramp is the time since the last measure
            measureRampMs += timeFromLastFrame;
            if (measureRampMs >= measureLengthMs) {
                measureRampMs -= measureLengthMs; // Reset the measure ramp precisely (not always 0)
            }
        }
    }
    public int getSection(float time) // Allow a time parameter to be passed in for offsets
    {
        for (int i = 0; i < sections.Count; i++) {
            if (time >= sectionStartTimes[i] && time < sectionEndTimes[i]) {
                return i;
            }
        }
        return -1; // will give out of range error
    }
}
