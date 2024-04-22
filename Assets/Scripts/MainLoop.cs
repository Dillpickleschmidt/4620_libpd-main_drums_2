using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLoop : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public SongTimer SongTimer;
    bool drumLoopToggle = false;
    bool bassToggle = false;

    void Start()
    {

    }

    void Update()
    {

        ExecuteWithinRange(5, 5, ref bassToggle, () =>
        {
            pdPatch.SendBang("bang_bass");
        });
        // ExecuteWithinRange(2, 3, ref bassToggle, () =>
        // {
        //     pdPatch.SendFloat("bass_pitch", pitch+0.05f);
        // });
        // ExecuteWithinRange(2, 3, ref drumLoopToggle, () =>
        // {
        //     pdPatch.SendBang("bang_drum_loop");
        // });
    }
    public void ExecuteWithinRange(int start, int end, ref bool toggle, Action startAction,  Action endAction = null)
    {
        if (SongTimer.getActiveSection() == start && !toggle)
        {
            startAction();  // Execute the passed logic
            toggle = true;
        }
        else if (SongTimer.getActiveSection() > end) {
            if (endAction != null) {
                endAction();
            }
            toggle = false;
        }
    }
}
