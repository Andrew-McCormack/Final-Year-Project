<?php
require_once "db_wordfrequencylookup.php";
    $word = htmlspecialchars($_GET["word"]);

    $sql = "SELECT `wordId` FROM `word` WHERE `word` = '$word'";

    $result = mysqli_query($db, $sql);
    $wordId = null;

    while($row = mysqli_fetch_assoc($result)) {
        $wordId = $row['wordId'];
    }

    if($wordId != null)
    {
        $sql = "SELECT * FROM `wordResponse` WHERE `wordId` = '$wordId'";
        $result = mysqli_query($db, $sql);

        $rows=array();
        while($row = mysqli_fetch_assoc($result)) {
            $rows[]=$row;
        }
        echo json_encode($rows);
    }
    else
    {
        echo "";
    }


?>