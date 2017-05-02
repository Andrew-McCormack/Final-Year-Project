<?php
require_once "db_wordfrequencylookup.php";
    $word = htmlspecialchars($_GET["word"]);
    $wordResponse = htmlspecialchars($_GET["wordResponse"]);
    $weight = htmlspecialchars($_GET["weight"]);
	$previousSubSize = htmlspecialchars($_GET["previousSubSize"]);
	$responseGroupId = htmlspecialchars($_GET["responseGroupId"]);
	
    $sql = "SELECT `wordId` FROM `word` WHERE `word` = '$word'";

    $result = mysqli_query($db, $sql);
    $wordId = null;
    while($row = mysqli_fetch_assoc($result)) {
        $wordId = $row['wordId'];
    }

    if($wordId == null) {
        $sql = "INSERT INTO `wordfrequencylookup`.`word` (`word`) 
		VALUES ('$word')";
        mysqli_query($db, $sql);
        echo "Successfully added record for word: " . $word;

        $sql = "SELECT `wordId` FROM `word` WHERE `word` = '$word'";
        $result = mysqli_query($db, $sql);

        while($row = mysqli_fetch_assoc($result)) {
            $wordId = $row['wordId'];
        }

        $sql = "INSERT INTO `wordfrequencylookup`.`wordResponse` (`wordResponse`, `weight`, `wordId`, `previousSubSize`, `responseGroupId`)
		VALUES ('$wordResponse', '$weight', '$wordId', $previousSubSize, $responseGroupId)";
        mysqli_query($db, $sql);

        echo "Successfully added record for wordResponse: " . $wordResponse;
    }

    else
    {

        $sql = "INSERT INTO `wordfrequencylookup`.`wordResponse` (`wordResponse`, `weight`, `wordId`, `previousSubSize`, `responseGroupId`)
		    VALUES ('$wordResponse', '$weight', '$wordId', $previousSubSize, $responseGroupId)";

        mysqli_query($db, $sql);

        echo "Successfully added record for wordResponse: " . $wordResponse;
        

    }
?>