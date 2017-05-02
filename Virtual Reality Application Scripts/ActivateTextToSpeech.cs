using System.Collections;
using VRTK;
using Valve.VR;
using System;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using UnityEngine;
using UnityEngine.UI;

public class ActivateTextToSpeech : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    [SerializeField]
    private bool m_ContinuousText = false;
    [SerializeField]
    private Text m_Output = null;
    [SerializeField]
    private InputField m_OutputAsInputField = null;
    [SerializeField]
    private Text m_OutputStatus = null;
    [SerializeField]
    private float m_MinConfidenceToShow = 0.5f;

    private string m_PreviousOutputTextWithStatus = "";
    private string m_PreviousOutputText = "";
    private float m_ThresholdTimeFromLastInput = 3.0f; //3 secs as threshold time. After 3 secs from last OnSpeechInput, we are considering input as new input
    private float m_TimeAtLastInterim = 0.0f;
    private bool micActive = false;

    private IBM.Watson.DeveloperCloud.Widgets.Widget.Input m_SpeechInput = new IBM.Watson.DeveloperCloud.Widgets.Widget.Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");

    private SpeechToText m_SpeechToText = new SpeechToText();

    // Use this for initialization
    void Start()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
    }

    void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        
        if(m_Output != null)
        {
            print("Testing");
            print(m_Output.text);
        }
        micActive = true;
    }

    private void OnSpeechInput(IBM.Watson.DeveloperCloud.Widgets.Widget.Data data)
    {
        if (m_Output != null || m_OutputAsInputField != null)
        {
            SpeechRecognitionEvent result = ((SpeechToTextData)data).Results;
            if (result != null && result.results.Length > 0)
            {
                string outputTextWithStatus = "";
                string outputText = "";

                if (Time.time - m_TimeAtLastInterim > m_ThresholdTimeFromLastInput)
                {
                    if (m_Output != null)
                        m_PreviousOutputTextWithStatus = m_Output.text;
                    if (m_OutputAsInputField != null)
                        m_PreviousOutputText = m_OutputAsInputField.text;
                }

                if (m_Output != null && m_ContinuousText)
                    outputTextWithStatus = m_PreviousOutputTextWithStatus;

                if (m_OutputAsInputField != null && m_ContinuousText)
                    outputText = m_PreviousOutputText;

                foreach (var res in result.results)
                {
                    foreach (var alt in res.alternatives)
                    {
                        string text = alt.transcript;
                        if (m_Output != null)
                        {
                            m_Output.text = string.Concat(outputTextWithStatus, string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
                        }

                        if (m_OutputAsInputField != null)
                        {
                            if (!res.final || alt.confidence > m_MinConfidenceToShow)
                            {
                                m_OutputAsInputField.text = string.Concat(outputText, " ", text);

                                if (m_OutputStatus != null)
                                {
                                    m_OutputStatus.text = string.Format("{0}, {1:0.00}", res.final ? "Final" : "Interim", alt.confidence);
                                }
                            }
                        }

                        if (!res.final)
                            m_TimeAtLastInterim = Time.time;

                    }
                }
            }
        }
    }
}