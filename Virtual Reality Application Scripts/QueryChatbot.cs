using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class QueryChatbot : MonoBehaviour {
    public List<string> wordList;
    public List<string> responseList;
    public bool isDone;
    public Animator animator;
    public IEnumerator routine;
    WatsonTextToSpeech textToSpeech;
    string response = "";

    void Start()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    public void startCORoutine()
    {
        isDone = false;
        responseList = new List<string>();
        print("In startcoroutine, wordListCount is " + wordList.Count);
        
        foreach (string word in wordList)
        {
            print(word);
            routine = TestNewRoutineGivesException(word);
            StartCoroutine(routine);
        }
        print("Finished coroutine, wordListCount is " + wordList.Count);
    }

    IEnumerator TestNewRoutineGivesException(string word)
    {
        // Comment out one of the urls depending on if the responses should be gotten locally or remotely
        //string url = "http://178.62.33.59/WordFrequencyLookup/GetResponses.php?word=" + word;
        string url = "http://localhost/WordFrequencyLookup/GetResponses.php?word=" + word;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.Send();
        while (!www.downloadHandler.isDone)
            yield return new WaitForEndOfFrame();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            responseList.Add(result);
        }

        yield break;
    }

    string LookForPerfectResponse()
    {
        isDone = true;
        List<List<WordResponse>> wordResponseListOfLists = new List<List<WordResponse>>();
        List<string> perfectResponseList = new List<string>();
        List<string> currentResponseList = new List<string>();
        bool firstRun = true;
        print("About to add to list");
        foreach (string response in responseList)
        {
            print("Adding responses to list of lists");
            List<WordResponse> data = new List<WordResponse>(JSONHelper.getJsonArray<WordResponse>(response));
            wordResponseListOfLists.Add(data);
        }

        foreach (List<WordResponse> wordResponseList in wordResponseListOfLists)
        {
            foreach (WordResponse wordResponse in wordResponseList)
            {
                if (firstRun && wordResponse.previousSubSize == wordList.Count)
                {
                    perfectResponseList.Add(wordResponse.wordResponse);
                }
                else if (wordResponse.previousSubSize == wordList.Count)
                {
                    currentResponseList.Add(wordResponse.wordResponse);
                }
            }
            if (firstRun)
            {
                firstRun = false;
            }
            else
            {
                perfectResponseList = perfectResponseList.Intersect(currentResponseList).ToList();
            }
        }

        if (perfectResponseList.Count != 0)
        {
            System.Random rnd = new System.Random();
            int r = rnd.Next(perfectResponseList.Count);
            return perfectResponseList[r];
        }

        return ("What?, I don't understand what you meant by that, could you speak slower?");
    }

    string GenerateResponse()
    {
        isDone = true;
        List<List<WordResponse>> wordResponseListOfLists = new List<List<WordResponse>>();
        var responseDistribution = new List<WordResponse>();
        var testDist = new Dictionary<string, List<ResponseWeightAndGroupId>>();
        List<ResponseWeightAndGroupId> responseValues = new List<ResponseWeightAndGroupId>();
        ResponseWeightAndGroupId responseWeightAndGroupId;
        float newValue;
        int responseId;
        bool isNewResponse;

        print("About to add to list");
        foreach (string response in responseList)
        {
            print("Adding responses to list of lists");
            List<WordResponse> data = new List<WordResponse>(JSONHelper.getJsonArray<WordResponse>(response));
            wordResponseListOfLists.Add(data);
        }
        int counter1 = 1;
        try
        {
            foreach (List<WordResponse> wordResponseList in wordResponseListOfLists)
            {
                print(wordResponseListOfLists.Count + " " + counter1);
                counter1++;
                foreach (WordResponse wordResponse in wordResponseList)
                {
                    if (wordResponse.weight < 0.7 && wordResponse.previousSubSize < wordList.Count + 2 && wordResponse.previousSubSize > wordList.Count - 2 || wordList.Count < 4)
                    {
                        responseValues = new List<ResponseWeightAndGroupId>();
                        if (testDist.ContainsKey(wordResponse.wordResponse))
                        {
                            responseValues.AddRange(testDist[wordResponse.wordResponse]);
                        }
                        newValue = wordResponse.weight;

                        responseId = wordResponse.responseGroupId;

                        isNewResponse = true;

                        if (responseValues.Count == 0)
                        {
                            responseWeightAndGroupId = new ResponseWeightAndGroupId();
                            responseWeightAndGroupId.weight = newValue;
                            responseWeightAndGroupId.responseGroupId = responseId;
                            responseValues.Add(responseWeightAndGroupId);
                            isNewResponse = false;
                        }
                        else
                        {
                            int count = 0;
                            foreach (ResponseWeightAndGroupId respWeightGroupId in responseValues)
                            {
                                
                                if (respWeightGroupId.responseGroupId == responseId)
                                {
                                    float updatedWeight = respWeightGroupId.weight + newValue;
                                    responseWeightAndGroupId = new ResponseWeightAndGroupId();
                                    responseWeightAndGroupId.responseGroupId = responseId;
                                    responseWeightAndGroupId.weight = updatedWeight;
                                    //responseValues.First(e => e.responseGroupId == responseId).weight = updatedWeight;

                                    responseValues[count] = responseWeightAndGroupId;
                                    isNewResponse = false;
                                }
                                count++;
                            }
                            if (isNewResponse)
                            {
                                responseWeightAndGroupId = new ResponseWeightAndGroupId();
                                responseWeightAndGroupId.responseGroupId = responseId;
                                responseWeightAndGroupId.weight = newValue;
                                responseValues.Add(responseWeightAndGroupId);
                            }
                        }

                        if (testDist.ContainsKey(wordResponse.wordResponse))
                        {
                            testDist[wordResponse.wordResponse] = responseValues;
                        }
                        else
                        {
                            testDist.Add(wordResponse.wordResponse, responseValues);
                        }
                    }
                }
                
            }
        }

        catch(Exception e)
        {
            print("Exception occured of type " + e);
        }

        string bestResponse = "whatever man";

        float bestResponseWeight = -1;

        List<string> perfectResponseList = new List<string>();

        /*foreach (KeyValuePair<string, List<ResponseWeightAndGroupId>> entry in testDist)
        {
            print(entry.Key);
            foreach(ResponseWeightAndGroupId respWeightGroupId in entry.Value)
            { 
                if(respWeightGroupId.weight >= bestResponseWeight)
                {
                    bestResponseWeight = respWeightGroupId.weight;
                    bestResponse = entry.Key;
                }
                if(respWeightGroupId.weight > 0.7)
                {
                    perfectResponseList.Add(entry.Key);
                }

            }
        }*/
        

        if (perfectResponseList.Count != 0)
        {
            System.Random rnd = new System.Random();
            int r = rnd.Next(perfectResponseList.Count);
            return perfectResponseList[r];
        }
        print(bestResponseWeight);
        return bestResponse;
    }

    void Update()
    {

        if (wordList.Count != 0 && responseList.Count == wordList.Count && !isDone)
        {
            animator.SetTrigger("WhenTalking");
            textToSpeech = gameObject.AddComponent<WatsonTextToSpeech>();
           // if (wordList.Count < 6)
            //{
            response = LookForPerfectResponse();
            print(response);
            textToSpeech.PlaySpeech(response);
            StopCoroutine(routine);
            /*
            }
            else if(wordList.Count >= 6 || response == "Could not find a suitable response")
            {
                print("Question too long, generating response");
                response = GenerateResponse();
                print(response);
                textToSpeech.PlaySpeech(response);
                StopCoroutine(routine);
            }*/
        }
        
    }
        
}
