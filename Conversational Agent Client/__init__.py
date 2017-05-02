#!/usr/bin/python
# -*- coding: utf-8 -*-
from gevent import monkey
import random
import requests
import urllib
from requests.packages.urllib3.util.retry import Retry
from requests.adapters import HTTPAdapter
import json
import WordResponse
import re
import BeautifulSoup
import simplejson as json
import logging
monkey.patch_all()
import sys

from flask import Flask, render_template, session, request, redirect, \
    url_for

from flask.ext.socketio import SocketIO, emit, join_room

app = Flask(__name__)
app.debug = True
app.config['SECRET_KEY'] = 'nuttertools'
socketio = SocketIO(app)


@app.route('/')
def chat():
    print('In chat')
    return render_template('chat.html')


@app.route('/login')
def login():
    print ('In login')
    return render_template('login.html')


@socketio.on('message', namespace='/chat')
def chat_message(message):
    print ('Returned from message')
    emit('message', {'data': message['data']}, broadcast=True)
    response = generateResponse(list(message['data'].values())[0])
    print (response)
    responseMessage = {'message': response, 'author': 'moviesubchatbot'}
    emit('message', {'data': responseMessage}, broadcast=True)


@socketio.on('connect', namespace='/chat')
def test_connect():
    emit('my response', {'data': 'Connected', 'count': 0})


def tokenize(input):
    input = input.lower()

    input = re.sub("'", '', input)
    input = re.sub('-', '', input)
    input = re.sub(r'[^A-Za-z0-9!?]+', ' ', input)

        # tokens = re.sub(r"[^\w\d'\s]+",'',input)

        # tokens = re.findall(r"\w+(?:[-']\w+)*|'|[-.(]+|\S\w*", input)

    return input.split()


def generateResponse(message):
    try:
        message = message.encode('utf-8')
        inputTokens = tokenize(message)
        print (inputTokens)
        perfectResponse = None

        if len(inputTokens) < 5:
            perfectResponse = attemptPerfectResponse(inputTokens)
            if perfectResponse:
                print ('Found perf')
                return perfectResponse

        s = requests.Session()

        # Taken from datashamen's solution at http://stackoverflow.com/questions/15431044/can-i-set-max-retries-for-requests-request
        # Default max retries led to pooling issues

        retries = Retry(total=5, backoff_factor=0.1,
                        status_forcelist=[500, 502, 503, 504])

        s.mount('http://', HTTPAdapter(max_retries=retries))

        print ('Could not find perf')
        responseDistribution = {}

    # Adding extra func

        inputSize = len(inputTokens)

    # print('Printing potential responses:')
    # Iterate through each word in the question

        for inputToken in inputTokens:
            print (inputToken)

        # Find every response associated with a particular word in the question

            tokenResponses = []
            url = 'http://localhost/WordFrequencyLookup/GetResponses.php?word=' + inputToken
            data = get_remote(url)
            if data is not None and data != '':
                try: 
                    if data != '':
                        data = json.loads(data.decode('utf-8'))

                        for index in range(0, len(data)):
                            response = data[index]['wordResponse']
                            weight = float(data[index]['weight'])
                            wordId = int(data[index]['wordId'])
                            previousSubSize = \
                                int(data[index]['previousSubSize'])
                            responseGroupId = \
                                int(data[index]['responseGroupId'])
                            wordResponse = \
                                WordResponse.WordResponse(response,
                                    weight, wordId, previousSubSize,
                                    responseGroupId)
                            tokenResponses.append(wordResponse)
 
                    # Iterate through every response associated with a particular word in the question

                        for response in tokenResponses:

                    # Added
                    # Find the number of word in a particular response

                            responseSize = \
                                len(tokenize(response.nextSubtitle))
                            if response.weight < 0.7 \
                                and response.previousSubSize \
                                < inputSize + 2 \
                                and response.previousSubSize \
                                > inputSize - 2 or inputSize < 5:

                    # If this response already exists in responseDistribution then we want to get the current
                    # value/weight associated with it, so it can be incremented as the response has been found again
                    # for another word

                                responseValues = responseDistribution.get(response.nextSubtitle)

                                newValue = response.weight
                                responseId = response.responseGroupId
                                isNewResponse = True
 
                                if responseValues == None:
                                    responseValues = []
                                    responseValues.append([newValue, responseId])
                                    isNewResponse = False
                                else:
                                    for (index, (totalWeight, respGroupId)) in enumerate(responseValues):
                                        if respGroupId == responseId:
                                            updatedValue = totalWeight + newValue
                                            responseValues[index] =  (updatedValue, responseId)
                                            isNewResponse = False
                                if isNewResponse:
                                    responseValues.append([newValue, responseId])

                                responseDistribution[response.nextSubtitle] =  responseValues
                except (ValueError, TypeError, IndexError, KeyError, e):
                    print (e)
                    print ('No response found')

        bestResponse = None
        bestResponseValue = -1
        almostPerfectResponseList = []
        perfectResponseList = []
 

        for (key, list) in responseDistribution.items():
            for (index, (totalWeight, groupId)) in enumerate(list):
                if totalWeight >= bestResponseValue:  
                    bestResponseValue = totalWeight
                    bestResponse = key
                elif totalWeight >= 0.99:
                    print ('Found a perfect response!' + str(value1))
                    perfectResponseList.append(key)
                elif totalWeight == 0.7:
                    almostPerfectResponseList.append(key)

        if len(perfectResponseList) != 0:
            return random.choice(perfectResponseList)   
        if len(almostPerfectResponseList) != 0:
            return random.choice(almostPerfectResponseList)

        return bestResponse
    except:  
        e = sys.exc_info()[0]
        print (e)
        return 'Error: ' + str(e)


def attemptPerfectResponse(inputTokens):
    perfectResponseList = []
    firstRun = True
    inputSize = len(inputTokens)

    s = requests.Session()

    # Taken from datashamen's solution at http://stackoverflow.com/questions/15431044/can-i-set-max-retries-for-requests-request
    # Default max retries led to pooling issues

    retries = Retry(total=5, backoff_factor=0.1, status_forcelist=[500,
                    502, 503, 504])
    s.mount('http://', HTTPAdapter(max_retries=retries))

    # Iterate through each word in the question

    for inputToken in inputTokens:
        print (inputToken)

        # Find every response associated with a particular word in the question

        tokenResponses = []
        url = \
            'http://localhost/WordFrequencyLookup/GetResponses.php?word=' \
            + inputToken
        data = get_remote(url)
        if data is not None and data != '':
            try:  
                if data != '':
                    data = json.loads(data.decode('utf-8'))
                    for index in range(0, len(data)):
                        response = data[index]['wordResponse']
                        weight = float(data[index]['weight'])
                        wordId = int(data[index]['wordId'])
                        previousSubSize = \
                            int(data[index]['previousSubSize'])
                        responseGroupId = \
                            int(data[index]['responseGroupId'])
                        wordResponse = \
                            WordResponse.WordResponse(response, weight,
                                wordId, previousSubSize,
                                responseGroupId)
                        tokenResponses.append(wordResponse)
                    currentResponseList = []

                    for response in tokenResponses:

                        if firstRun and response.previousSubSize == inputSize:
                            perfectResponseList.append(response.nextSubtitle)
                        elif response.previousSubSize == inputSize:
                            currentResponseList.append(response.nextSubtitle)
                    if firstRun:
                        firstRun = False
                    else:
                        perfectResponseList = set(perfectResponseList) & set(currentResponseList)
            except (ValueError, TypeError, IndexError, KeyError, e):
                print (e)
                print ('No response found')

    if perfectResponseList:
        return ''.join(random.sample(perfectResponseList, 1))

    return None


def get_remote(url, attempt=0):
    try:
        print ('Attempt ' + str(attempt))
        request = urllib.urlopen(url)
        response = request.read()
        return response
    except (urllib2.HTTPError, error):
        if error.code in (403, 404):
            if attempt < 5:
                return get_remote(url, attempt=attempt + 1)
        raise


if __name__ == '__main__':

        try:
            socketio.run(app, host='0.0.0.0', port=8080)
        except:
            e = sys.exc_info()[0]
            print (e)


            