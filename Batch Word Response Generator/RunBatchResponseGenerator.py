import RemoteBatchWordResponseGen
import os
import Tokenize
import random
import requests
import simplejson as json
import WordResponse

#NUM_FILES is used for setting the number of files that will be read into the wordResponse hashmap
PREV_NUM_FILES = 602
NUM_FILES = 702
wordFrequencyDistribution  = {}

def main():
    print("Beginning batch word-response generation process, please ensure database has been correctly set up as laid out in SetupLocalEnvironmentIntructions.txt and that the .srt files are stored in  the 'Subs' subdirectory")
    initialize()
    print("Finished batch word-response generation!")


def initialize():

    try:
        subtitleFilesDirectory = os.path.join('./Subs')
        print(subtitleFilesDirectory)
        subtitleList = []

        for filename in sorted(os.listdir(subtitleFilesDirectory)):
            print(filename)
            subtitleList.append(filename)

        for i in subtitleList:
            RemoteBatchWordResponseGen.RemoteBatchWordResponseGen(subtitleFilesDirectory + '/' + i)
    except Exception as e:
        print("An error occured while running the batch word-response generation process, please ensure database is "
              "set up correctly as described in SetupLocalEnvironmentIntructions.txt and that .srt files are stored in the 'Subs' subdirectory")
        print(e)

if __name__ == "__main__":
    main()