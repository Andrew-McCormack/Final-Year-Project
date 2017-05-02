class WordResponse:
    nextSubtitle = ''
    weight = 0.0
    id = None
    previousSubSize = None
    responseGroupId = None
    

    def __init__(self, nextSubtitle, weight, id, previousSubSize, responseGroupId):
        self.nextSubtitle = nextSubtitle
        self.weight = weight
        self.id = id
        self.previousSubSize = previousSubSize
        self.responseGroupId = responseGroupId