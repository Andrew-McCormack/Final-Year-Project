import re

class Tokenize:

    def tokenize(input):
        input = input.lower()

        input = re.sub("'", "", input)
        input = re.sub("-", "", input)
        input = re.sub(r'[^A-Za-z0-9!?]+', ' ', input)
        #tokens = re.sub(r"[^\w\d'\s]+",'',input)


        #tokens = re.findall(r"\w+(?:[-']\w+)*|'|[-.(]+|\S\w*", input)
        return input.split()