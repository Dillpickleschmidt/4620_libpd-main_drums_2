using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My_DrumSequencer : MonoBehaviour
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
            StepsObjs[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StepsObjs[i].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            StepsObjs[i].transform.position = new Vector3(i * 1.5f, 0, 0);
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
            envelopes[i] = ControlFunctions.ADSR(ramp/2000, gates[i][count], adsr_params);
        }
        // if (sounds.Count > 0) {
        //     envelopes[sounds.Count-1] = ControlFunctions.ADSR(ramp/3000, gates[sounds.Count-1][count], adsr_params);
        // }
        if (trig)
        {
            StartCoroutine(SendMidi(count));
            count = (count + 1) % kick.Count;

        }
        
        for (int i = 0; i < sounds.Count-1; i++) // ignore the last sound
        {
            if(gates[i][count])
                StepsObjs[count].transform.localScale = new Vector3(0.75f+(0.5f-envelopes[i]*0.5f), 0.75f+(0.5f-envelopes[i]*0.5f), 0.75f+(0.5f-envelopes[i]*0.5f));
        }
        if (gates[gates.Length-1][count]) // last sound
            StepsObjs[count].transform.position = new Vector3(StepsObjs[count].transform.position.x, 3f-envelopes[gates.Length-1]*3, 0);
    }
}
