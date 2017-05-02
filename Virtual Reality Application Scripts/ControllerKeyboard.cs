using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerKeyboard : MonoBehaviour {

    SteamVR_TrackedObject obj;
    public GameObject keyboardHolder;
    public GameObject displayHolder;
    public bool buttonEnabled;


	// Use this for initialization
	void Start () {
        obj = GetComponent<SteamVR_TrackedObject>();
        keyboardHolder.SetActive(false);
        displayHolder.SetActive(true);
        //uiInteractionHolder.SetActive(false);
        buttonEnabled = false;
	}
	
	void Update () {
        var device = SteamVR_Controller.Input((int)obj.index);

        if(device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            if(!buttonEnabled)
            {
                Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.name == "Standard (Instance)")
                    {
                        renderers[i].enabled = false;
                    }
                }
                keyboardHolder.SetActive(true);
                buttonEnabled = true;
                //uiInteractionHolder.SetActive(true);
            }
            else
            {
                Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.name == "Standard (Instance)")
                    {
                        renderers[i].enabled = true;
                    }
                }
                keyboardHolder.SetActive(false);
                buttonEnabled = false;
                //uiInteractionHolder.SetActive(false);
            }
            
        }
	}
}
