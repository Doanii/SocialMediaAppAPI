"use strict";

//const { post } = require("jquery");

const activityTypeStyles = {
    0: { color: "#4da6ff", text: `<i class="fa-solid fa-arrow-up-from-bracket"></i> Posten` },
    1: { color: "#ff8585", text: `<i class="fa-solid fa-heart"></i>  Liken` },
    2: { color: "#85daff", text: `<i class="fa-solid fa-comment"></i> Comments` },
    3: { color: "#ffbe4f", text: `<i class="fa-solid fa-user-plus"></i> Volgen` },
    4: { color: "#91ffc5", text: `<i class="fa-solid fa-address-card"></i> Account` }
};

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
    connection.invoke("UserJoinsPerDay").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("NewPostReceived").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("MostPopulairPosts").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("MostPopularUsers").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("DisplayActivities").catch(function (err) {
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