"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/DashboardHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

const InvokeData = () => {
    connection.invoke("TotalPosts").catch(function (err) {
        return console.error(err.toString());
    });
}

const start = async () => {
    connection.start().then(function () {
        console.log("SignalR Connected.");
        InvokeData();
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.onclose(async () => {
    await start();
});

start();

connection.on("TotalPosts", (count) => {
    console.log(count);
    const element = document.getElementById("TotalPostsCount");
    element.innerHTML = count;
})