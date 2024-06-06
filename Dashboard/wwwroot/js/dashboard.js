"use strict";

//const { post } = require("jquery");

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
    connection.invoke("NewPostReceived").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("MostPopulairPosts").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("MostPopularUsers").catch(function (err) {
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

connection.on("MostPopularUsers", (users) => {
    console.log(users);
    const userContainer = document.getElementById("mostActiveUsers");
    userContainer.innerHTML = ""; // Clear the previous posts

    users.forEach(user => {
        const postElement = document.createElement("div");
        postElement.className = "bg-white p-3 rounded flex gap-6";

        postElement.innerHTML = `
            <div>
                <div>
                    <span class="font-bold">@${user.userName}</span>
                </div>
                <div>
                    <span><i class="fa-solid fa-clock-rotate-left pr-2"></i>${user.activityCount} ${user.activityCount > 1 ? "activiteiten" : "activiteit"}</span>
                </div>
            </div>
        `;

        userContainer.appendChild(postElement);
    });
})

connection.on("NewPostReceived", (posts) => {
    const postContainer = document.getElementById("postsContainer");
    postContainer.innerHTML = ""; // Clear the previous posts

    posts.forEach(post => {
        const postElement = document.createElement("div");
        postElement.className = "bg-white p-3 rounded flex gap-6";

        postElement.innerHTML = `
            <div>
                <div>
                    <span class="font-bold">@${post.opUsername}</span>
                    <span class="font-thin timestamp" data-timestamp="${post.createdAt}">  (${formatTimeAgo(new Date(post.createdAt))})</span>
                </div>
                <div>
                    ${post.content}
                </div>
                <div>
                    <span class="pr-3"><i class="fa-solid fa-heart" style="color: red;"></i> ${post.likeCount}</span>
                    <span class="pr-3"><i class="fa-solid fa-comment" style="color: #4f90ff"></i> ${post.commentCount}</span>
                </div>
            </div>
        `;

        postContainer.appendChild(postElement);
    });

    // Update timestamps every second
    setInterval(updateTimestamps, 1000);
});

connection.on("MostPopulairPosts", (posts) => {
    const postContainer = document.getElementById("mostPopulairPosts");
    postContainer.innerHTML = ""; // Clear the previous posts

    posts.forEach(post => {
        const postElement = document.createElement("div");
        postElement.className = "bg-white p-6 rounded flex gap-6";

        postElement.innerHTML = `
            <div>
                <div>
                    <span class="font-bold">@${post.opUsername}</span>
                    <span class="font-thin timestamp" data-timestamp="${post.createdAt}">  (${formatTimeAgo(new Date(post.createdAt))})</span>
                </div>
                <div>
                    ${post.content}
                </div>
                <div>
                    <span class="pr-3"><i class="fa-solid fa-heart" style="color: red;"></i> ${post.likeCount}</span>
                    <span class="pr-3"><i class="fa-solid fa-comment" style="color: #4f90ff"></i> ${post.commentCount}</span>
                </div>
            </div>
        `;

        postContainer.appendChild(postElement);
    });

    // Update timestamps every second
    setInterval(updateTimestamps, 1000);
});

function formatTimeAgo(date) {
    const now = Date.now();
    const nowUTC = new Date(now).getTime() + new Date().getTimezoneOffset() * 60 * 1000;
    const diffInSeconds = Math.floor((nowUTC - date) / 1000);

    if (diffInSeconds < 60) {
        return diffInSeconds === 1 ? "1 seconde geleden" : `${diffInSeconds} seconden geleden`;
    } else if (diffInSeconds < 3600) {
        const diffInMinutes = Math.floor(diffInSeconds / 60);
        return diffInMinutes === 1 ? "1 minuut geleden" : `${diffInMinutes} minuten geleden`;
    } else if (diffInSeconds < 86400) {
        const diffInHours = Math.floor(diffInSeconds / 3600);
        return `${diffInHours} uur geleden`;
    } else {
        const diffInDays = Math.floor(diffInSeconds / 86400);
        return diffInDays === 1 ? "1 dag geleden" : `${diffInDays} dagen geleden`;
    }
}

function updateTimestamps() {
    const timestamps = document.querySelectorAll(".timestamp");
    timestamps.forEach(timestampElement => {
        const timestamp = new Date(timestampElement.getAttribute("data-timestamp"));
        timestampElement.innerText = `  (${formatTimeAgo(timestamp)})`;
    });
}