
using UnityEngine;

/// Script to set the y-axis position of a GameObject based an amplitude value sent to use from PD.
public class EnvelopeFollower : MonoBehaviour
{
	/// The PD patch we are going to listen to.
	public LibPdInstance pdPatch;

	///	The Transform of the GameObject we are going to move.
	public Transform moveObject;

    ///	We need to tell pdPatch which PD send object we want to listen to.
    void Start()
    {
        //Bind to the named send object.
		pdPatch.Bind("AmplitudeEnvelope");
    }

	///	Clean up after ourselves and unbind from the AmplitudeEnvelope object.
	///	(not strictly necessary in this code, but a good habit to get into)
	void OnDestroy()
	{
		//Unbind from the named send object.
		//pdPatch.UnBind("AmplitudeEnvelope");
	}

	///	Our receive function. This will be called whenever a bound send object
	///	in our PD patch fires.
	public void FloatReceive(string sender, float value)
	{
		//This function will get called for *every* Float event sent by our
		//patch, so we need to make sure we're only acting on the
		//*AmplitudeEnvelope* event that we're actually interested in.
		if (sender == "AmplitudeEnvelope")
		{
			
			moveObject.position = new Vector3(moveObject.position.x,
											  0.5f + value,
											  moveObject.position.z);
		}
	}
}
