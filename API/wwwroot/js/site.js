"use strict";
var token="eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InNhcnlhLnR1bGFpbWF0QG91dGxvb2suY29tIiwiZ2l2ZW5fbmFtZSI6IlNhcnlhVHVsYWltYXQiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2NDY1MDcwODcsImV4cCI6MTY0NzExMTg4NywiaWF0IjoxNjQ2NTA3MDg3LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDE3LyJ9.nBAB6HVgTR5hiNuo0ohGJabLZfXotWp30xk8n1MK4-jCpJc8-kzxTdW8RYHloJhceDzzhKnLwPH15aB0UI367g"
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/post",{ accessTokenFactory: () => token })
    .build();
connection.start().then(function () {}).catch(function (err) {
    return console.error(err.toString());
});
connection.on("NotfiyPostadded", function (message) { 
    console.log(message);
});
document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("post", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});