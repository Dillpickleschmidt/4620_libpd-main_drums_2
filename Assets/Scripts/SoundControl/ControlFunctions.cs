using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ControlFunctions 
{
    static public float Sin(float t, float freq, float phase)
    {
        float ang = t * freq * Mathf.PI * 2;
        return Mathf.Sin(ang + phase * Mathf.PI * 2);
    }
    static public float Tri(float t, float freq, float phase)
    {
        float a = (t * freq + phase) % 1 * 2;
        if (a > 1)
            a = 1 - (a - 1);
        return a * 2 - 1;
    }
    static public float Saw(float t, float freq, float phase)
    {
        float a = (t * freq + phase) % 1;
        return a * 2 - 1;
    }
    static public float Squ(float t, float freq, float phase)
    {
        float a = Saw(t, freq, phase);
        if (a < 0) { a = -1; }
        else { a = 1; }
        return a;
    }
    
    static public float InterpLfo(float t, float freq, float amp, float phase, float shape)
    {
        float inter = shape % 1;
        int sh = Mathf.FloorToInt(shape);
        float lf1 = 0;
        float lf2 = 0;
        if (sh == 3)
            return Squ(t, freq, phase);
        if (sh == 0)
        {
            lf1 = Sin(t, freq, phase);
            lf2 = Tri(t, freq, phase);
        }
        if (sh == 1)
        {
            lf1 = Tri(t, freq, phase);
            lf2 = Saw(t, freq, phase);
        }
        if (sh == 2)
        {
            lf1 = Saw(t, freq, phase);
            lf2 = Squ(t, freq, phase);
        }
        return Mathf.Lerp(lf1, lf2, inter)*amp;
    }
    static public float[] fourFO(float t, float freq, float amp, float shape)
    {
        float[] lfos = new float[4];
        lfos[0] = InterpLfo(t, freq,amp, 0, shape);
        lfos[1] = InterpLfo(t, freq,amp, 0.25f, shape);
        lfos[2] = InterpLfo(t, freq,amp, 0.5f, shape);
        lfos[3] = InterpLfo(t, freq,amp, 0.75f, shape);
        return lfos;
    }
    
    static public int[] PitchArray( int key, Vector2Int range, int[] mode)
    {
        List<int> pList = new List<int>();
        int offset = 12 * Mathf.FloorToInt(range.x / 12);
        int octave = 0;
        int pitch = 0;
        while (pitch < range.y)
        {
            int accum = key;
            int tonic = key + offset + octave;
            pList.Add(tonic );
            for (int i = 0; i < mode.Length; i++)
            {
                accum += mode[i];
                pitch = accum + octave + offset;
                pList.Add(pitch );
            }
            octave += 12;

        }
        //foreach (var p in pList.ToArray())
          //  Debug.Log(p);
        return pList.ToArray();
    }
    public static Vector2Int PitchRange(float offset, float scale)
    {
        int offs = Mathf.FloorToInt(offset * 127);
        return new Vector2Int(offs, Mathf.FloorToInt(scale * 127) + offs);
    } 

    public static float ADSR(float t, bool gate, Vector4 par)
    {
        float A = par.x / 1000;
        float D = par.y / 1000;
        float S = par.z;
        float R = par.w / 1000;
        float env = 0;
        if (!gate && t > (A + D + R))
        {
            return 0;
        }
            
        else if (t < A && gate)
        {
            env = t / A;
        }
        else if (t > A && t < (A+D) && gate)
        {
            env = Mathf.Lerp(1, S, (t - A) / D);
        }
        else if (t > (A + D) && gate)
        {
            env = S;
        }
        else if (!gate && t < R)
        {
            env = Mathf.Lerp(S, 0, t / R);
        }
        return env;
    }

}
