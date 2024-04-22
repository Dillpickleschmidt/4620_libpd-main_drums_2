using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
  public LibPdInstance patch;
  float ramp;
  float t;
  int[] mode;
  int count = 0;
    // Add a toggle button in Unity
  public bool toggle;
  [SerializeField]
  List<bool> steps;

  // Start is called before the first frame update
  void Start()
  {
      mode = new int[]{2,2,3,2,2};
      // Set the toggle to false
      toggle = false;
  }

  // Update is called once per frame
  void Update()
  {
      t += Time.deltaTime;
      float lfo = ControlFunctions.Sin(t, 0.2345f, 0);
      int[] PitchArray = ControlFunctions.PitchArray(0, new Vector2Int(24, 48), mode);
      int pitch_index = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (PitchArray.Length-1));

      int pitch = PitchArray[pitch_index];

      bool trig = ramp > (ramp + Time.deltaTime) % 1;
      ramp = (ramp + Time.deltaTime) % 1;
      float env = Mathf.Pow(1 - ramp, 7);

  if (trig) {
    if (steps[count]) {
      patch.SendMidiNoteOn(0, pitch, 80);
    }
    count = (count + 1) % steps.Count;
    Debug.Log(count);
  }
      transform.position = new Vector3(lfo * 4, env*3, 0);
  }
}
