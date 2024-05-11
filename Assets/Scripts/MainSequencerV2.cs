using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSequencerV2 : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public SongTimerV2 SongTimerV2;
    public GameObject objectTemplate; // Object to change material settings
    Material material;
    Color originalColor;
    Color newColor;
    float colorChangeDuration = 0.225f;
    float colorChangeTimer = 0f;
    bool kicked = false;

    // Sequencer settings
    [SerializeField] float bassVolume = 1f;
    [SerializeField] float kickVolume = 0.7f;
    [SerializeField] float hihat1Volume = 0.5f;
    [SerializeField] float hihat2Volume = 0.5f;
    [SerializeField] bool[] sectionsKick = new bool[16];
    [SerializeField] bool[] sectionsHihat1 = new bool[16];
    [SerializeField] bool[] sectionsHihat2 = new bool[16];
    [SerializeField] bool[] sectionsBass = new bool[16];
    [SerializeField] bool[] kick = new bool[16];
    [SerializeField] bool[] hihat1 = new bool[16];
    [SerializeField] bool[] hihat2 = new bool[16];
    [SerializeField] bool[] bass = new bool[32];
    [SerializeField] int[] bassPitchPattern = new int[32];
    float bassPitch = 0.125f;
    [SerializeField] int kickLoopDuration = 1; // 1 measure
    [SerializeField] int hihat1LoopDuration = 1; // 1 measure
    [SerializeField] int hihat2LoopDuration = 1; // 1 measure
    [SerializeField] int bassLoopDuration = 16; // 2 measures
    float[] cMajorScaleScaled;

    // Reverb settings
    float originalReverbAmount = 0.7f;

    float reverbAmount = 0.7f;
    int reverbOutput = 100;
    int reverbLiveliness = 60;
    int reverbCrossoverFreq = 3000;
    int reverbHighFreqDamping = 40;

    // Local ramp variables
    float timeDifference1;
    bool started1 = false;
    float startTime1;

    bool swellTrigger;

    // Create instrument object containing all instrument settings
    public class Instrument
    {
        public string name;
        public bool[] pattern;
        public int loopDuration;
        public bool[] sections;
        public float rampMs = 0;
        public int[] pitch;
    }

    // Create an array of instrument objects
    Instrument[] instruments = new Instrument[4]; // UPDATE THIS WHEN ADDING MORE INSTRUMENTS!

    void Start()
    {
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
            material = objectTemplate.GetComponent<Renderer>().material;
            originalColor = material.GetColor("_EmissionColor");
            newColor = originalColor * 2f;
        }

        // Initialize instrument objects
        instruments[0] = new Instrument
        {
            name = "kick",
            pattern = kick,
            loopDuration = kickLoopDuration,
            sections = sectionsKick,
        };

        instruments[1] = new Instrument
        {
            name = "hihat1",
            pattern = hihat1,
            loopDuration = hihat1LoopDuration,
            sections = sectionsHihat1,
        };

        instruments[2] = new Instrument
        {
            name = "hihat2",
            pattern = hihat2,
            loopDuration = hihat2LoopDuration,
            sections = sectionsHihat2,
        };
        instruments[3] = new Instrument
        {
            name = "bass",
            pattern = bass,
            loopDuration = bassLoopDuration,
            sections = sectionsBass,
            pitch = bassPitchPattern,
        };

        // Send initial values to the Pure Data patch
        pdPatch.SendFloat("reverb_amount", reverbAmount);
        pdPatch.SendFloat("reverb_output", reverbOutput);
        pdPatch.SendFloat("reverb_liveliness", reverbLiveliness);
        pdPatch.SendFloat("reverb_crossover_freq", reverbCrossoverFreq);
        pdPatch.SendFloat("reverb_high_freq_damping", reverbHighFreqDamping);
        pdPatch.SendFloat("kick_volume", kickVolume);
        pdPatch.SendFloat("hihat1_volume", hihat1Volume);
        pdPatch.SendFloat("hihat2_volume", hihat2Volume);
        pdPatch.SendFloat("bass_volume", bassVolume);
        pdPatch.SendFloat("bass_pitch", bassPitch);
    }

    // Update is called once per frame
    void Update()
    {
        if (SongTimerV2.started)
        {
            for (int i = 0; i < instruments.Length; i++)
            {
                // Each instrument might have its own measure length based on loop duration
                float instrumentMeasureMs = instruments[i].loopDuration * SongTimerV2.measureLengthMs;

                // Calculate the exact index for the pattern based on the current time
                float currentCyclePositionMs = instruments[i].rampMs % instrumentMeasureMs;
                int patternIndex = Mathf.FloorToInt((currentCyclePositionMs / instrumentMeasureMs) * instruments[i].pattern.Length);

                // Check if it's time to move to the next division and if there's a note at that division
                float nextCyclePositionMs = (instruments[i].rampMs + SongTimerV2.timeFromLastFrame) % instrumentMeasureMs;
                int nextPatternIndex = Mathf.FloorToInt((nextCyclePositionMs / instrumentMeasureMs) * instruments[i].pattern.Length);

                // Check active section and pattern change
                if (instruments[i].sections[SongTimerV2.getSection(SongTimerV2.t)])
                {
                    if (nextPatternIndex != patternIndex && instruments[i].pattern[nextPatternIndex])
                    {
                        pdPatch.SendBang("bang_" + instruments[i].name);
                        Debug.Log("Bang " + instruments[i].name + " at index " + nextPatternIndex);
                        if (instruments[i].name == "kick")
                        {
                            // lower the brightness of the objectTemplate's emission for 200 ms
                            material.SetColor("_EmissionColor", newColor);
                            kicked = true;
                            // Debug.Log("Magenta");
                        }
                    }
                    // Specific logic for instruments like bass that may require pitch changes
                    if (instruments[i].name == "bass")
                    {
                        int pitchIndex = patternIndex; // Use the current patternIndex for bass pitch
                        pdPatch.SendFloat("bass_pitch", cMajorScaleScaled[instruments[i].pitch[pitchIndex]]);
                    }

                    // Update the ramp time
                    instruments[i].rampMs += SongTimerV2.timeFromLastFrame;
                    if (instruments[i].rampMs >= instrumentMeasureMs)
                    {
                        instruments[i].rampMs -= instrumentMeasureMs;
                    }
                }
            }
            if (kicked) {
                colorChangeTimer += Time.deltaTime;
                if (colorChangeTimer > colorChangeDuration) {
                    material.SetColor("_EmissionColor", originalColor);
                    kicked = false;
                    colorChangeTimer = 0f;
                    // Debug.Log("Yellow");
                }
            }
            // send 'bang_swell' message to Pure Data patch 3 sec before the end of section 4
            if (SongTimerV2.t >= SongTimerV2.sectionStartTimes[4] - 3800)
            {
                if (!swellTrigger)
                {
                    swellTrigger = true;
                    pdPatch.SendBang("bang_swell");
                    Debug.Log("SWELL SENT");
                }
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
        Debug.Log("timeDifference: " + timeDifference);
        return timeDifference;
    }
}
