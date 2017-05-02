using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class ChangeScene : MonoBehaviour
{

    public int currLevel = 0;
    public string[] levelNames = new string[2] { "aircraft level", "jungggld" };
    public GameObject generateButton;

    static ChangeScene s = null;

    // Use this for initialization
    void Start()
    {
        if (s == null)
        {
            s = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            currLevel = (currLevel + 1) % 2;
            SteamVR_LoadLevel.Begin(levelNames[currLevel]);
        }
    }
}
