using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using UnityEngine.SceneManagement;

public class WatsonTextToSpeech : MonoBehaviour
{
    TextToSpeech m_TextToSpeech = new TextToSpeech();
    //string m_TestString = "Hello! This is Text to Speech!";
    string m_ExpressiveText = "<speak version=\"1.0\"><prosody pitch=\"150Hz\">Hello! This is the </prosody><say-as interpret-as=\"letters\">IBM</say-as> Watson <express-as type=\"GoodNews\">Unity</express-as></prosody><say-as interpret-as=\"letters\">SDK</say-as>!</speak>";
    public AudioSource source;

    void Start()
    {
        print("In watson start");
        string sceneName = SceneManager.GetActiveScene().name;
        print("About to get scene name");
        print(sceneName);
        print(sceneName.Substring(0, sceneName.IndexOf(" ")));
        print("After");
        if(sceneName.Substring(0, sceneName.IndexOf(" ")) == "John")
        {
            print("Getting Michael");
            m_TextToSpeech.Voice = VoiceType.en_US_Michael;
        }
        else
        {
            print("Getting Kate");
            m_TextToSpeech.Voice = VoiceType.en_GB_Kate;
        }

        //m_TextToSpeech.Voice = VoiceType.en_GB_Kate;
        //m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback);

        //m_TextToSpeech.Voice = VoiceType.en_US_Allison;
        //m_TextToSpeech.ToSpeech(m_ExpressiveText, HandleToSpeechCallback);
    }
    

    public void PlaySpeech(string speech)
    {
        m_TextToSpeech.ToSpeech(speech, HandleToSpeechCallback);
    }

    void HandleToSpeechCallback(AudioClip clip)
    {
        PlayClip(clip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            GameObject audioObject = new GameObject("AudioObject");
            source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.loop = false;
            source.clip = clip;
            source.Play();
           
            GameObject.Destroy(audioObject, clip.length);
        }
    }
}