using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polyrhythm : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public SongTimerV2 SongTimerV2;
    public int numNotes;
    float deltaTimeMs;
    int beatLengthMs;
    // Create a list of lists to store the play times for each note
    public List<List<float>> playTimes = new List<List<float>>();
    float originalReverbAmount = 0.8f;
    float reverbAmount = 0.8f;
    int reverbOutput = 100;
    int reverbLiveliness = 90;
    int reverbCrossoverFreq = 3000;
    int reverbHighFreqDamping = 30;
    float delayFeedbackAmount = 0.2f;
    float polyrhythm_volume = 0.35f;
    public float[] polyrhythmIntervals = new float[35];

    // Local ramp variables
    float timeDifference1;
    bool started1 = false;
    float startTime1;
    float timeDifference2;
    bool started2 = false;
    float startTime2;
    float timeDifference3;
    bool started3 = false;
    float startTime3;

    void Start()
    {
        numNotes = 35;
        beatLengthMs = 60000 / 60; // 1000ms
        pdPatch.SendFloat("reverb_amount", reverbAmount);
        pdPatch.SendFloat("reverb_output", reverbOutput);
        pdPatch.SendFloat("reverb_liveliness", reverbLiveliness);
        pdPatch.SendFloat("reverb_crossover_freq", reverbCrossoverFreq);
        pdPatch.SendFloat("reverb_high_freq_damping", reverbHighFreqDamping);
        pdPatch.SendFloat("delay_feedback_amount", delayFeedbackAmount);
        pdPatch.SendFloat("polyrhythm_volume", polyrhythm_volume);

        for (int i = numNotes; i > 0; i--)
        {
            float currentNoteInterval = beatLengthMs + (numNotes - i) * 2.232f;
            polyrhythmIntervals[i-1] = currentNoteInterval;
            Debug.Log("Note " + (numNotes - i) + " has " + polyrhythmIntervals[i - 1] + "ms interval");

            int totalBeats = Mathf.RoundToInt(450 * 1000 / currentNoteInterval);
            List<float> notePlayTimes = new List<float>();

            // Properly calculate each note's play times
            for (int j = 0; j < totalBeats; j++)
            {
                float playTime = currentNoteInterval * j;
                notePlayTimes.Add(playTime);
            }

            playTimes.Add(notePlayTimes);
            Debug.Log("Note " + i + " has " + totalBeats + " beats with intervals starting at " + currentNoteInterval);
        }
    }

    void Update()
    {
        deltaTimeMs = SongTimerV2.t;
        for (int i = 0; i < numNotes; i++)
        {
            int bang = i + 1;
            // Debug.Log("Note " + i + " has a first beat at " + playTimes[i][0] + "ms");
            if (deltaTimeMs >= playTimes[i][0]) {
                // Send a bang message to the Pure Data patch
                pdPatch.SendBang("bang_" + bang);
                // Debug.Log("bang_" + (i+1));
                playTimes[i].RemoveAt(0);
            }
        }
        if (SongTimerV2.started) {
            // raise reverb
            if (SongTimerV2.t >= SongTimerV2.sectionStartTimes[5] - 9000 && SongTimerV2.t < SongTimerV2.sectionEndTimes[4])
            {
                float rampLength = 9000;
                float rampTime = TimeRamp(SongTimerV2.t, rampLength, ref started1, ref startTime1, ref timeDifference1);
                // map rampTime from 0 to 3000 to 0 to 1
                float rampValue = rampTime / rampLength;
                pdPatch.SendFloat("reverb_amount", originalReverbAmount * (1+rampValue*1f));
                // pdPatch.SendFloat("reverb_amount", originalReverbAmount * (1 + Mathf.Sign(rampValue) * rampValue * rampValue));
                // Debug.Log("Ramp value: " + rampValue);
            }
            // remove reverb
            if (SongTimerV2.t >= SongTimerV2.sectionStartTimes[5] - 0 && SongTimerV2.t < SongTimerV2.sectionEndTimes[5])
            {
                float rampLength = 200;
                float rampTime = TimeRamp(SongTimerV2.t, rampLength, ref started2, ref startTime2, ref timeDifference2);
                // map rampTime from 0 to 200 to 0 to 1
                float rampValue = 1 - (rampTime / rampLength);
                pdPatch.SendFloat("reverb_amount", originalReverbAmount * rampValue);
                // Debug.Log("Ramp value: " + rampValue);
            }
            // raise reverb
            if (SongTimerV2.t >= SongTimerV2.sectionStartTimes[6] && SongTimerV2.t < SongTimerV2.sectionEndTimes[6])
            {
                float rampLength = 15000;
                float rampTime = TimeRamp(SongTimerV2.t, rampLength, ref started3, ref startTime3, ref timeDifference3);
                // map rampTime from 0 to 3000 to 0 to 1
                float rampValue = rampTime / rampLength;
                pdPatch.SendFloat("reverb_amount", originalReverbAmount * (1+rampValue*1f));
                // pdPatch.SendFloat("reverb_amount", originalReverbAmount * (1 + Mathf.Sign(rampValue) * rampValue * rampValue));
                // Debug.Log("Ramp value: " + rampValue);
            }
        }
    }
    float TimeRamp(float currentTime, float allowedTimeLength, ref bool started, ref float startTime, ref float timeDifference) {
        if (started == false)
        {
            startTime = currentTime;
            started = true;
        }
        timeDifference = currentTime - startTime;
        if (timeDifference > allowedTimeLength)
        {
            // clamp the timeDifference to the allowedTimeLength
            timeDifference = allowedTimeLength;
        }
        // Debug.Log("timeDifference: " + timeDifference);
        return timeDifference;
    }
};
