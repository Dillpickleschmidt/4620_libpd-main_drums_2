using System;
using UnityEngine;

public class Polyrhythm_Backup : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public int numNotes = 36;
    private float[] nextPlayTime;
    private float baseFrequency = 1f;  // Base frequency in Hz (1 Hz = 1 beat per second)
    private float globalVolume = 1f;   // Initial global volume
    private float volumeDecayRate = 0.98f;  // Adjust decay rate for volume drop-off
    private int baseFrequencyCount = 0;  // Counter for the base frequency beat
    bool resetVolume = false;

    void Start()
    {
        nextPlayTime = new float[numNotes];
        for (int i = 0; i < numNotes; i++)
        {
            nextPlayTime[i] = Time.time + 1.0f;  // Initial delay for each note
        }
        pdPatch.SendFloat("global_volume", globalVolume);  // Initialize global volume in Pd
    }

    void Update()
    {
        float currentTime = Time.time;
        resetVolume = false;

        for (int i = numNotes-1; i >= 0; i--)
        {
            if (currentTime >= nextPlayTime[i])
            {
                // Send a bang to the Pure Data patch corresponding to this note
                pdPatch.SendBang("bang_" + (i + 1));

                // Calculate the next time to play this note based on frequency
                float frequency = baseFrequency - i * 0.00222f; // Dynamic frequency adjustment
                nextPlayTime[i] = currentTime + (1 / frequency);
                // Check if this is the base note (i.e., the first one with baseFrequency)
                if (i == 0) {
                    baseFrequencyCount++;  // Increment the base frequency counter
                    Debug.Log($"Base frequency beat occurred {baseFrequencyCount} times.");
                    // FULLY LOOPS AT 251 BEATS
                    // Spike the volume at the start of any cycle
                    resetVolume = true;
                }
            }
        }
        // if (resetVolume) {
        //     globalVolume = 1.0f; // Spike the volume at the start of base frequency cycle
        // } else {
        //     // Decay the volume over time
        //     globalVolume = Mathf.Max(0.5f, globalVolume - (float)Math.Pow(Time.deltaTime, volumeDecayRate));
        // }

        pdPatch.SendFloat("global_volume", globalVolume);
    }
}
