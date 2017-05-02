from operator import eq
import Subtitle
import Tokenize
import WordResponse
from chardet.universaldetector import UniversalDetector
import re
from nltk.corpus import stopwords
import requests
from requests.packages.urllib3.util.retry import Retry
from requests.adapters import HTTPAdapter

responseGroupId = 1
class RemoteBatchWordResponseGen():

    currentSubtitle = None
    nextSubtitle = None

    def __init__(self, subtitleFile):
        self.analyse(subtitleFile)

    def analyse(self, subtitleFile):
        global currentSubtitle
        global nextSubtitle

        global mostCommonWords

        global responseGroupId

        print(responseGroupId)

        firstRun = True

        try:
            print('Attempting to open ' + subtitleFile)
            subFile = open(subtitleFile, 'r')
            #subFile = codecs.open(subtitleFile, 'r', encoding=encodingEstimate)

        except Exception as e:
            print('Could not open, wrong encoding perhaps?')

        firstRun = True

        while (True):
            try:
                subNumber = subFile.readline()
                subNumber = subNumber.strip()
                if ((subNumber == '') or (subNumber.isspace()) or (not(subNumber.isdigit()))):
                    break
                else:

                    try:
                        timeSub = subFile.readline()

                        if(timeSub == None):
                            raise ValueError('Error parsing time subtitle')

                        subtitleString = ''
                        s = None
                        newSubtitleFound = False
                        nextSubtitleText = ''
                        while (True):
                            s = subFile.readline()
                            if((s == '') or (s.isspace())):
                                break
                            else:
                                if ((s.strip().startswith('-')) and (len(subtitleString) > 0)):
                                    newSubtitleFound = True
                                    nextSubtitleText = s
                                else:
                                    subtitleString += s + ' '

                        startTime = int(self.parse(timeSub.split('-->')[0]))
                        stopTime = int(self.parse(timeSub.split('-->')[1]))

                        #print(startTime)
                        #print(stopTime)

                        number = int(subNumber)

                        nextSubtitle = Subtitle.Subtitle(number, startTime, stopTime, subtitleString)


                        if(firstRun == True):
                            firstRun = False
                            currentSubtitle = nextSubtitle
                        else:
                            self.checkSubtitlesAreOk()
                            if(newSubtitleFound == True):
                                nextSubtitle = Subtitle.Subtitle(number, startTime, stopTime, nextSubtitleText)
                                self.checkSubtitlesAreOk()

                    except Exception as e:
                        print('Could not parse sub, exception was ' + str(e))
                    #return frequencyDistribution
            except Exception as e:
                print('Could not parse sub file, exception was ' + str(e))

    def detectEncoding(self, subFile):
        detector = UniversalDetector()

        try:
            subFile = open(subFile, 'rb')

            for line in subFile:
                detector.feed(line)
                if detector.done:
                    break
            detector.close()

        except Exception as e:
            print('Could not open' + str(e))
            return 0
        return str(detector.result).split(',')[1]


    def isTimeOk(self):
        MAX_TIME_DIFFERENCE = 2000
        #print('Current sub is: ' + currentSubtitle.subtitleString)
        #print('Next sub is: ' + nextSubtitle.subtitleString)

        global currentSubtitle
        global nextSubtitle
        global responseGroupId
        nextStartTime = nextSubtitle.startTime
        #print(nextStartTime)
        currEndTime = currentSubtitle.stopTime
        #print(currEndTime)
        diff = nextStartTime - currEndTime

        #print('Result is: ' + str((diff < MAX_TIME_DIFFERENCE)).lower())

        if(diff < MAX_TIME_DIFFERENCE):
            return True

        return False

    def subtitlesAreOk(self):
        global currentSubtitle
        global nextSubtitle
        global responseGroupId

        nextSub = nextSubtitle.subtitleString
        subNumber = nextSubtitle.subtitleNumber

        if (currentSubtitle.subtitleString[0] == '#'):
            currentSubtitle.subtitleString = currentSubtitle.subtitleString[1:]
        if (nextSubtitle.subtitleString[0] == '#'):
            nextSubtitle.subtitleString = nextSubtitle.subtitleString[1:]

        if(currentSubtitle == None):
            return False
        elif(nextSub == None):
            return False
        elif(eq(nextSub.strip(), '')):
            return False
        elif("[" in nextSub.strip() or "]" in nextSub.strip()):
            return False
        elif("<" in nextSub.strip() or ">" in nextSub.strip()):
            return False
        elif("srt" in nextSub):
            return False
        elif ("http://" in nextSub):
            return False
        elif("Subtitles by" in nextSub):
            return False
        elif(subNumber == 9999):
            return False

        #print('Returning true')
        return True


    def parse(self, input):
        split = []
        split = (input.split(':'))

        hours = int(split[0].strip())
        minutes = int(split[1].strip())

        secondsMillies = split[2].split(','.strip())

        seconds = int(secondsMillies[0])
        millies = int(secondsMillies[1])

        return hours * 60 * 60 * 1000 + minutes * 60 * 1000 + seconds * 1000 + millies;


    def checkSubtitlesAreOk(self):
        global currentSubtitle
        global nextSubtitle
        global reaponseGroupId

        if(self.isTimeOk()):
            if(self.subtitlesAreOk()):
                self.addToFrequencyDistribution(currentSubtitle.subtitleString, nextSubtitle.subtitleString)
                currentSubtitle = nextSubtitle
                nextSubtitle = None
        else:
            currentSubtitle = nextSubtitle

        nextSubtitle = None

    def tokenize(self, text):
        text = text.lower()
        text = re.sub(r"[^\w\d'\s]+", '', input)

        tokens = re.findall(r"\w+(?:[-']\w+)*|'|[-.(]+|\S\w*", text)

        return tokens

    def addToFrequencyDistribution(self,currSubtitleText, nextSubtitleText):
        global responseGroupId

        tokens = Tokenize.Tokenize.tokenize(currSubtitleText)


        nextSubtitleText = " ".join(nextSubtitleText.split())

        if (nextSubtitleText.endswith(".")):
            nextSubtitleText = nextSubtitleText[:-1]

        size = int(len(tokens))

        notStopWords = []
        stopWords = []
        stop = set(stopwords.words('english'))
        for i in tokens:
            if i not in stop:
                notStopWords.append(i)
            else:
                stopWords.append(i)

        stopWordCount = size - int(len(notStopWords))
        notStopWordCount = size - int(len(stopWords))

        if (notStopWordCount > 0):
            notStopWordWeight = 0.7 / notStopWordCount
        if (stopWordCount > 0):
            stopWordWeight = 0.3 / stopWordCount
        defaultWeight = 1.0 / float(size)

        s = requests.Session()

        # Taken from datashamen's solution at http://stackoverflow.com/questions/15431044/can-i-set-max-retries-for-requests-request
        # Default max retries led to pooling issues

        retries = Retry(total=5,
                        backoff_factor=0.1,
                        status_forcelist=[500, 502, 503, 504])

        s.mount('http://', HTTPAdapter(max_retries=retries))

        if (notStopWordCount != 0):
            for notStopWord in notStopWords:
                #print("Popping notStopWord")

                data = {"word" : notStopWord, "wordResponse" : nextSubtitleText, "weight" : notStopWordWeight,
                        "previousSubSize" : size, "responseGroupId" : responseGroupId}
                r = s.post("http://localhost/WordFrequencyLookup/AddRecord.php",
                                  params=data)

        if(stopWordCount != 0):
            for stopWord in stopWords:
                data = {"word": stopWord, "wordResponse": nextSubtitleText, "weight": stopWordWeight,
                        "previousSubSize": size, "responseGroupId": responseGroupId}
                r = s.post("http://localhost/WordFrequencyLookup/AddRecord.php",
                                  params=data)

        responseGroupId += 1