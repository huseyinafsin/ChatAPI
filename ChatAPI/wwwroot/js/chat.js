"use strict";
var token = document.getElementById("tokenInput").value;
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub",
    {
        accessTokenFactory: () => token
    }
).build();
document.getElementById("sendButton").disabled = true;

console.log(token);
console.log(connection.AddressInfo);

connection.on("ReceiveMessage",
    function(user, message) {

        var li = document.createElement("li");
        document.getElementById("messagesList").appendChild(li);

        li.textContent = `${user} says ${message}`;
    });

connection.start().then(function() {
    document.getElementById("sendButton").disabled = false;
}).catch(function(err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click",
    function(event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function(err) {
            return console.error(err.toString());
        });

        event.preventDefault();
    });