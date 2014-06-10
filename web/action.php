<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
</head>
<body>

<?php
// mysqli database interface
$success = false;
// mysqli(mysql_host_address, login_name, login_password, initial_database)
$mysqli = new mysqli('localhost', 'games', 'password', 'games');
if ($mysqli->connect_errno) {
    echo "Failed to connect to mysql:(" . $mysqli->connect_errno .")" . $mysql->connect_error;
}
else {
    if (!($stmt = $mysqli->prepare("insert into users(id, password, name, birthday, email, qq, mobilephone, signature, registertime) values(null, ?, ?, ?, ?, ?, ?, ?, ?)"))) {
        echo "Prepare failed:(" . $mysqli->errno .")" . $mysqli->error;
    }
    else {
        $registertime = date("Y-m-d H:i:s");//just now in UTC
        $password = hash("sha1", $_POST['password'], false);//sha1sum of password
        if (!$stmt->bind_param("ssssssss", $password, $_POST['name'], $_POST['birthday'], $_POST['email'], $_POST['qq'], $_POST['mobilephone'], $_POST['signature'], $registertime)) {
            echo "Binding parameters failed:(" . $stmt->errno . ")" . $stmt->error;
        }
        else {
            if (!$stmt->execute()) {
                echo "Execute failed:(" . $stmt->errno . ")" . $stmt->error;
            }
            else {
                $id = $mysqli->insert_id;
                $success = true;
                $mysqli->close();
            }
        }
    }
}
?>
<?php
if ($success) {
?>
你好，<?php echo htmlspecialchars($_POST['name']); ?>。 <br />
你已经成功注册，你的通行证ID为：<?php echo $id; ?> ，请牢记。
<?php
}
?>
</body>
</html>
