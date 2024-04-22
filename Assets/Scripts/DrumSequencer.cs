using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSequencer : MonoBehaviour
{
    [SerializeField] int beat;
    float t;
    public LibPdInstance pdPatch;
    float ramp;
    int count = 0;
    [SerializeField]
    List<bool> kick;
    [SerializeField]
    List<bool> snare;
    [SerializeField]
    List<bool> cymbal;
    public List<AudioClip> sounds;
    string[] drum_type = new string[] { "Kick", "Snare", "Cymbals" };
    List<float> envelopes;
    List<bool>[] gates = new List<bool>[3];
    Vector4 adsr_params;
    GameObject[] StepsObjs;
    void Start()
    {
        envelopes = new List<float>();
        StepsObjs = new GameObject[kick.Count];
        for (int i = 0; i < sounds.Count; i++)
        {
            string name = sounds[i].name + ".wav";
            pdPatch.SendSymbol(drum_type[i], name);
            envelopes.Add(0);

        }
        for (int i = 0; i < kick.Count; i++)
        {
            StepsObjs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            StepsObjs[i].transform.position = new Vector3(i * 1.5f, 1, -1);
        }
        gates[0] = kick;
        gates[1] = snare;
        gates[2] = cymbal;
        adsr_params = new Vector4(100, 150, .8f, 500);
        
    }
    IEnumerator SendMidi(int count)
    {
        if (kick[count])
        {
            pdPatch.SendBang("kick_bang");
        }
        if (snare[count])
        {
            pdPatch.SendBang("snare_bang");
        }
        if (cymbal[count])
        {
            pdPatch.SendBang("cymbal_bang");
        }
        yield return null;
    }


    void Update()
    {
        t += Time.deltaTime;
        int dMs = Mathf.RoundToInt(Time.deltaTime * 1000);
        bool trig = ramp > ((ramp + dMs) % beat);
        ramp = (ramp + dMs) % beat;

        for (int i = 0; i < sounds.Count; i++)
        {
            envelopes[i] = ControlFunctions.ADSR(ramp/1000, gates[i][count], adsr_params);
        }
        if (trig)
        {
            StartCoroutine(SendMidi(count));
            count = (count + 1) % kick.Count;

        }
        
        for (int i = 0; i < sounds.Count; i++)
        {
            if(gates[i][count])
                StepsObjs[count].transform.position = new Vector3(count * 1.5f, envelopes[i], 0);
        }

    }
}
