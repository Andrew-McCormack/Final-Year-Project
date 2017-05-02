using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Tokenizer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public List<string> Tokenize(string input)
    {
        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        input = rgx.Replace(input, "");
        input = input.Replace(",", "");
        input = input.Replace("'", "");
        input = input.Replace("-", "");

        List<string> splitInput = new List<string>(input.Split(null));
        return splitInput;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
