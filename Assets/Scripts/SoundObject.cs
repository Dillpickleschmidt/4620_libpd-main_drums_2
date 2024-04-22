using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public LibPdInstance patch;
    int ramp;
    float t;
    [SerializeField] int beat;
    int[] pitchArr;
    [Range(0,1000)]
    [SerializeField] int A;
    [Range(0, 1000)]
    [SerializeField] int D;
    [Range(0f, 1f)]
    [SerializeField] float S;
    [Range(0, 1000)]
    [SerializeField] int R;

    void Start()
    {
        pitchArr = ControlFunctions.PitchArray(0, new Vector2Int(48, 60), new int[] { 2, 1, 2, 2, 2, 1 });
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        int dMs = Mathf.RoundToInt(Time.deltaTime * 1000);
        float lfo = ControlFunctions.Sin(t, 0.1522f, 0);
       
        int pitch_ind = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (pitchArr.Length-1));
        bool trig = ramp > ((ramp + dMs) % beat);
        ramp = (ramp + dMs) % beat;
        if(trig)
        {
            patch.SendMidiNoteOn(0, pitchArr[pitch_ind], 60);
        }

        Vector4 adsr_par = new Vector4(A, D, S, R);
        float gate_len = beat/2;
        patch.SendList("ADSR", gate_len, A, D, S, R);
        
        bool gate = ramp < gate_len/100f;
        float adsr = ControlFunctions.ADSR((float)ramp / beat, gate, adsr_par);
        transform.position = new Vector3(0, adsr, 0);
    }
}
