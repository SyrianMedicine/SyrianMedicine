"use strict";

function connect() {
    var token = document.getElementById("token").value;
    
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/post", {
            accessTokenFactory: () => token
        }).withAutomaticReconnect()
        .build();
    connection.start().then(function () {}).catch(function (err) {
        return console.error(err.toString());
    });
    connection.on("NotfiyPostadded", function (message) {
        console.log("server: " + message);
    });
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var message = document.getElementById("messageInput").value;
        connection.invoke("post", message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
}
document.getElementById("connect").addEventListener("click", connect);