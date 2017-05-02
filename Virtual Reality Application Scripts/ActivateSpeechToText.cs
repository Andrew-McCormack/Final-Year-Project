
using IBM.Watson.DeveloperCloud.Widgets;
using Normal.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

#pragma warning disable 414

public class ActivateSpeechToText : MonoBehaviour
{
        private SteamVR_TrackedObject trackedObj;
        private SteamVR_Controller.Device device;
        public GameObject speechToTextHolder;
        public GameObject displayHolder;
        public GameObject controllerKeyboardHolder;
        public GameObject animationHolder;
    public QueryChatbot queryChatbot;
    public SpeechDisplayWidget speechDisplayWidget;
        public KeyboardDisplay keyboardDisplay;
        public ControllerKeyboard controllerKeyboard;

    // Use this for initialization
    void Start()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        speechDisplayWidget = speechToTextHolder.GetComponent(typeof(SpeechDisplayWidget)) as SpeechDisplayWidget;
        keyboardDisplay = displayHolder.GetComponent(typeof(KeyboardDisplay)) as KeyboardDisplay;
        controllerKeyboard = controllerKeyboardHolder.GetComponent(typeof(ControllerKeyboard)) as ControllerKeyboard;
    }

    void Update()
    {
        string speechToTextOutput = speechDisplayWidget.getOutput();
        if (speechToTextOutput.Length > 0 && !controllerKeyboard.buttonEnabled)
        {
            keyboardDisplay._text.text = speechToTextOutput.Substring(0, speechToTextOutput.IndexOf(" (")).Trim();
        }
    }

    void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        string currentDisplay = keyboardDisplay._text.text;
        if (currentDisplay != "" && !controllerKeyboard.buttonEnabled)
        {
            print(currentDisplay);
            Tokenizer tokenizer = gameObject.AddComponent<Tokenizer>();
            List<string> splitInput = tokenizer.Tokenize(currentDisplay);

            queryChatbot = animationHolder.GetComponent<QueryChatbot>() as QueryChatbot;
            queryChatbot.wordList = new List<string>(splitInput);
            queryChatbot.startCORoutine();

            keyboardDisplay._text.text = "";
        }
    }
    
}
