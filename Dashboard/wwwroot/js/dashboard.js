"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/DashboardHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

const InvokeData = () => {
    connection.invoke("TotalPosts").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("NewPostsToday").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("UserCount").catch(function (err) {
        return console.error(err.toString());
    });
};

const start = async () => {
    connection.start().then(function () {
        console.log("SignalR Connected.");
        InvokeData();
    }).catch(function (err) {
        return console.error(err.toString());
    });
};

connection.onclose(async () => {
    await start();
});

start();

connection.on("TotalPosts", (count) => {
    document.getElementById("TotalPostsCount").innerHTML = count;
});

connection.on("NewPostsToday", (count) => {
    document.getElementById("NewPostsToday").innerHTML = count;
});

connection.on("UserCount", (count) => {
    document.getElementById("UserCount").innerHTML = count;
});