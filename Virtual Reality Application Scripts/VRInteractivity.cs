using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental;
using UnityEngine.Networking;

public class VRInteractivity : MonoBehaviour {
    // Make SteamVR controller device, stores a reference to the controller that will be used
    private SteamVR_Controller.Device controller;


    // Use this for initialization
    void Start () {
        // Get the tracked object of the hand
        SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();

        // Get the reference to the controller using the index of the tracked object
        controller = SteamVR_Controller.Input((int)trackedObj.index);
    }
	
	// Update is called once per frame
	void Update () {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) // force grab
        {
            StartCoroutine(GetText());
        }
    }

    IEnumerator GetText()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://178.62.33.59/WordFrequencyLookup/GetResponses.php?word=hello"))
        {
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }
            else // Success
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
