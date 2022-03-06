"use strict";

function connect() {
    var token = document.getElementById("token").value;
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/Publichub", {
            accessTokenFactory: () => token
        }).withAutomaticReconnect()
        .build();
    connection.start().then(function () {}).catch(function (err) {
        return console.error(err.toString());
    });
    connection.on("NotfiyUserFollowYou", function (user, messege) {
        console.log(user);
    });
    connection.on("NotfiyPostCreated", function (post, messege) {
        console.log(post);
    });
}
document.getElementById("connect").addEventListener("click", connect);