using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polyrhythm : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public SongTimerV2 SongTimerV2;
    int numNotes;
    float deltaTimeMs;
    int beatLengthMs;
    // Create a list of lists to store the play times for each note
    public List<List<float>> playTimes = new List<List<float>>();
    float reverbAmount = 0.8f;
    int reverbOutput = 100;
    int reverbLiveliness = 90;
    int reverbCrossoverFreq = 3000;
    int reverbHighFreqDamping = 30;
    float delayFeedbackAmount = 0.2f;
    float polyrhythm_volume = 0.35f;
    public float[] polyrhythmIntervals = new float[35];

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

        // loop through the number of notes from last to first
        for (int i = numNotes; i > 0; i--)
        {
            bool started = false;
            float currentNoteInterval = beatLengthMs + (numNotes - i) * 2.232f;
            polyrhythmIntervals[i-1] = currentNoteInterval;
            // Debug.Log("Note " + (numNotes - i) + " has " + polyrhythmIntervals[i - 1] + "ms interval");
            int totalBeats = Mathf.RoundToInt(450 * 1000 / currentNoteInterval);
            List<float> notePlayTimes = new List<float>();
            for (int j = 1; j <= totalBeats; j++)
            {
                if (!started) {
                    notePlayTimes.Add(0); // first value is 0ms
                    started = true;
                }
                notePlayTimes.Add(currentNoteInterval * j);
            }
            playTimes.Add(notePlayTimes);
            // Debug.Log("Note " + i + " has " + totalBeats + " beats");
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
    }
};
