"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, sentAt) {
    var li = document.createElement("li");
    
    li.textContent = `${user}: ${message} sent at ${sentAt}`;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;

    if (message.trim() === "") { // Check if the message is empty
        alert("Message cannot be empty.");
        return;
    }

    var targetCustomerIdInput = document.getElementById("targetCustomerId");
    var targetCustomerId = targetCustomerIdInput ? targetCustomerIdInput.value : null;

        connection.invoke("SendMessage", message,targetCustomerId).catch(function (err) {
            return console.error(err.toString());
        });

    document.getElementById("messageInput").value = ""; // Clear the input field
    

    event.preventDefault();
});
