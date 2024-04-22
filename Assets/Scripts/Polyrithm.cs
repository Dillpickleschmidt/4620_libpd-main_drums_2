using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polyrhythm : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public SongTimer SongTimer;
    int numNotes;
    int deltaTimeMs;
    int beatLengthMs;
    // Create a list of lists to store the play times for each note
    List<List<float>> playTimes = new List<List<float>>();
    float reverbAmount = 0.5f;
    int reverbOutput = 100;
    int reverbLiveliness = 80;
    int reverbCrossoverFreq = 3000;
    int reverbHighFreqDamping = 20;
    float delayFeedbackAmount = 0.2f;

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

        // loop through the number of notes from last to first
        for (int i = numNotes; i > 0; i--)
        {
            bool started = false;
            float currentNoteInterval = beatLengthMs + (numNotes - i) * 2.232f;
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
        deltaTimeMs = SongTimer.deltaTimeMs;
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
