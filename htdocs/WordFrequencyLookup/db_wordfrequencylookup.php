<?php
$db = mysqli_connect('localhost', 'root', '1234');
if ( $db === FALSE ) die('Fail message');
if ( mysqli_select_db($db, "wordfrequencylookup") === FALSE ) die("Could not access db");
?>