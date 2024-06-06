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
    connection.invoke("UserJoinsPerDay").catch(function (err) {
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

connection.on("UserJoinsPerDay", (userjoins) => {
    const ctx = document.getElementById('myChart');
    
    const dates = userjoins.map(x => new Date(x.date).toLocaleDateString('en-US', { day: 'numeric', month: 'short' }));
    const counts = userjoins.map(x => x.count);
    
    console.log(userjoins)

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: dates,
            datasets: [{
                label: '# of users joined',
                data: counts,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    ticks: {
                        stepSize: 1
                    },
                    beginAtZero: true
                }
            }
        }
    });
});
