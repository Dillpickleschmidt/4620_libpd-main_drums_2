using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSequencer : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public SongTimer SongTimer;
    [SerializeField]
    bool[] sectionsKick = new bool[16];
    [SerializeField]
    bool[] sectionsHihat1 = new bool[16];
    [SerializeField]
    bool[] sectionsHihat2 = new bool[16];
    bool[][] instruments = new bool[3][];
    [SerializeField]
    bool[] kick = new bool[16];
    [SerializeField]
    bool[] hihat1 = new bool[16];
    [SerializeField]
    bool[] hihat2 = new bool[16];
    [SerializeField]
    int[] bassPattern = new int[32];
    [SerializeField]
    bool[] whoosh1 = new bool[32];
    float ramp;
    int count = 0;
    float deltaMeasure;
    int previousSection = 0;
    float previousSectionLastMeasure; // last measure of the previous section
    float bassPitch = 0.125f;
    bool[] activeInstruments = new bool[3];
    float[] cMajorScaleScaled;
    float reverbAmount = 0.8f;
    int reverbOutput = 100;
    int reverbLiveliness = 90;
    int reverbCrossoverFreq = 3000;
    int reverbHighFreqDamping = 30;
    bool whoosh1Trig = false;
    
    void Start()
    {
        // UPDATE THIS WHEN ADDING MORE INSTRUMENTS!
        instruments[0] = sectionsKick;
        instruments[1] = sectionsHihat1;
        instruments[2] = sectionsHihat2;

        // E major scale MIDI numbers
        // C major scale MIDI numbers
        float[] cMajorScaleMidi = { 60, 61.5f, 63.125f, 64, 66, 68.125f, 70.5f, 72 }; // C, D, E, F, G, A, B, C
        cMajorScaleScaled = new float[cMajorScaleMidi.Length];

        for (int i = 0; i < cMajorScaleMidi.Length; i++)
        {
            // Subtract the MIDI number for E (64) to make E the "root note" (0 in the original scale)
            // Then divide by the range of MIDI numbers in the E major scale (12) to scale to 0-1
            // Then divide by 8 to scale to 0-0.125
            // Then add 0.125 to make E (the root note) equal to 0.125
            cMajorScaleScaled[i] = ((cMajorScaleMidi[i] - 60f) / 12.0f / 8.0f) + 0.125f;
        }

        pdPatch.SendFloat("reverb_amount", reverbAmount);
        pdPatch.SendFloat("reverb_output", reverbOutput);
        pdPatch.SendFloat("reverb_liveliness", reverbLiveliness);
        pdPatch.SendFloat("reverb_crossover_freq", reverbCrossoverFreq);
        pdPatch.SendFloat("reverb_high_freq_damping", reverbHighFreqDamping);
    }

    IEnumerator SendMidi(int count, bool[] activeInstruments)
    {
        if (kick[count] && activeInstruments[0])
        {
            pdPatch.SendBang("bang_kick");
        }
        if (hihat1[count] && activeInstruments[1])
        {
            pdPatch.SendBang("bang_hihat1");
        }
        if (hihat2[count] && activeInstruments[2])
        {
            pdPatch.SendBang("bang_hihat2");
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (SongTimer.started)
        {
            bool trig = ramp > ((ramp + SongTimer.timeFromLastFrame) % (SongTimer.beatLengthMs/4));
            ramp = (ramp + SongTimer.timeFromLastFrame) % (SongTimer.beatLengthMs/4);
            // Main sequencers (sequence over 1 measure)
            if (trig) {
                // Get active instruments for the current section
                for (int i = 0; i < instruments.Length; i++) {
                    activeInstruments[i] = instruments[i][SongTimer.getActiveSection()];
                }
                // Play the active instruments
                StartCoroutine(SendMidi(count, activeInstruments));
                count = (count + 1) % kick.Length;
                // Debug.Log("Count: " + count);
            }
            
            // Sequence measure counter
            if (trig)
            {
                if (SongTimer.getActiveSection() != previousSection) {
                    previousSectionLastMeasure = SongTimer.measureCounter; // last measure of the previous section
                    Debug.Log("Section " + previousSection + " last measure: " + previousSectionLastMeasure);
                }
                previousSection = SongTimer.getActiveSection();
                deltaMeasure = SongTimer.measureCounter - previousSectionLastMeasure;
                // Debug.Log("Delta measure: " + deltaMeasure);

            }

            // Bass sequencer
            float sectionLength = SongTimer.sections[SongTimer.getActiveSection()];
            // Scale the delta measure to the length of the bass pattern and use that as the index for the pitch
            int pitchIndex = bassPattern[(int)Math.Floor(deltaMeasure*(bassPattern.Length / sectionLength))];
            // Debug.Log("Measure * 2 = pitch index " + deltaMeasure*2 + "* 2 " + "= " + pitchIndex);
            float pitchScaled = cMajorScaleScaled[pitchIndex];
            pdPatch.SendFloat("bass_pitch", pitchScaled);
            // Debug.Log("Bass pitch: " + pitchScaled);

            if (!whoosh1Trig) {
                // Hardcoded whoosh1 section of 4
                if (SongTimer.getActiveSection() == 4) {
                    if (whoosh1[(int)Math.Floor(deltaMeasure*(whoosh1.Length / sectionLength))])
                    {
                        pdPatch.SendBang("bang_whoosh1");
                        whoosh1Trig = true;
                    }
                }
            }
        }

    }
}
