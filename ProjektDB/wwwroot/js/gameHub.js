const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

connection.start()
    .then(() => {
        console.log("SignalR-anslutning upprättad!");
    })
    .catch(err => console.error("SignalR-fel:", err));

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
});

connection.on("ShipPlaced", (data) => {
    console.log(`Skepp placerat: ${JSON.stringify(data)}`);
    // Uppdatera brädet visuellt med det placerade skeppet
    updateBoard(data.userId, data.startX, data.startY, data.endX, data.endY, data.shipType);
});

connection.on("ShotFired", (data) => {
    console.log(`Skott avfyrat: (${data.targetX}, ${data.targetY})`);
    // Uppdatera brädet visuellt med resultatet av skottet
    updateShotResult(data.userId, data.targetX, data.targetY, data.hit);
});

connection.on("StatisticsUpdated", (stats) => {
    StatisticsUpdate(stats);
});

function PlayerJoined(gameId) {
    connection.invoke("JoinGame", gameId)
        .then(() => {
            console.log(`Gick med i spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid att gå med i spelet:", err));
}

function LeaveGame(gameId) {
    connection.invoke("LeaveGame", gameId)
        .then(() => {
            console.log(`Lämnade spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid att lämna spelet:", err));
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

// Visuella uppdateringar

function initializeBoard(boardId) {
    const board = document.getElementById(boardId);
    for (let i = 0; i < 10; i++) {
        for (let j = 0; j < 10; j++) {
            const cell = document.createElement('div');
            cell.classList.add('board-cell');
            cell.dataset.x = i;
            cell.dataset.y = j;
            board.appendChild(cell);
        }
    }
}

window.onload = () => {
    initializeBoard('playerBoard');
    initializeBoard('opponentBoard');
};

function updateBoard(userId, startX, startY, endX, endY, shipType) {
    const boardId = userId === 1 ? 'playerBoard' : 'opponentBoard'; // Antag att 1 är spelaren, och 2 är motståndaren
    const board = document.getElementById(boardId);

    const ship = document.createElement('div');
    ship.classList.add('ship');
    ship.style.gridColumnStart = startX + 1;
    ship.style.gridRowStart = startY + 1;
    ship.style.gridColumnEnd = endX + 1;
    ship.style.gridRowEnd = endY + 1;
    ship.setAttribute('data-ship-type', shipType);

    board.appendChild(ship);
}

function updateShotResult(userId, targetX, targetY, hit) {
    const boardId = userId === 1 ? 'opponentBoard' : 'playerBoard'; // Uppdatera motståndarens bräde vid skott
    const board = document.getElementById(boardId);

    const shotResult = document.createElement('div');
    shotResult.classList.add('shot');
    shotResult.style.gridColumnStart = targetX + 1;
    shotResult.style.gridRowStart = targetY + 1;
    shotResult.classList.add(hit ? 'hit' : 'miss');

    board.appendChild(shotResult);
}

document.getElementById('placeShipsBtn').addEventListener('click', () => {
    // Skicka platsinformation för att placera ett skepp
    const startX = 2, startY = 3, endX = 4, endY = 3, shipType = 'Battleship';
    PlaceShip(1, startX, startY, endX, endY, shipType); // Antag att 1 är spelaren
});

document.getElementById('fireShotBtn').addEventListener('click', () => {
    // Skicka skottinformation
    const targetX = 3, targetY = 3;
    FireShot(1, targetX, targetY); // Antag att 1 är spelaren
});
