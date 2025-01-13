const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub") 
    .build();

connection.start()
    .then(() => {
        console.log("SignalR-anslutning upprättad!");
        // bekräftelse?
    })
    .catch(err => console.error("SignalR-fel:", err));

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
    //behövs denna?
});

function PlayerJoined(gameId) {
    connection.invoke("JoinGame", gameId)
        .then(() => {
            console.log(`Gick med i spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid att gå med i spelet:", err));
        //annan UI-förändring?
}
function LeaveGame(gameId) {
    connection.invoke("LeaveGame", gameId)
        .then(() => {
            console.log(`Lämnade spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid att lämna spelet:", err));
        //annan UI-förändring?
}

function StatisticsUpdate(stats) {
    document.querySelector("#matchesPlayed").innerText = stats.MatchesPlayed;
    document.querySelector("#matchesWon").innerText = stats.MatchesWon;
    document.querySelector("#matchesLost").innerText = stats.MatchesLost;
    document.querySelector("#winPercentage").innerText = `${stats.WinPercentage}%`;
}

function FireShot(gameId, targetX, targetY) {
    connection.invoke("FireShot", gameId, targetX, targetY)
        .then(() => {
            console.log(`Skott avfyrat mot (${targetX}, ${targetY}) i spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid avfyrning av skott:", err));
}

function PlaceShip(gameId, startX, startY, endX, endY, shipType) {
    connection.invoke("ShipPlaced", gameId, startX, startY, endX, endY, shipType)
        .then(() => {
            console.log(`Skepp av typen "${shipType}" placerat från (${startX}, ${startY}) till (${endX}, ${endY}) i spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid placering av skepp:", err));
}

//allt som finns i hubben måste synkas med js för att göra visuella förändringar/bekräftelser eller kunna samarbeta med spelaren.
