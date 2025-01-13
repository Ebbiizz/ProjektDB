const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

connection.start()
    .then(() => {
        console.log("SignalR-anslutning upprättad!");
    })
    .catch(err => console.error("SignalR-fel:", err));

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
    initializeBoard('player-board');
    initializeBoard('opponent-board');
};

const shipLengths = {
    Carrier: 5,
    Battleship: 4,
    Cruiser: 3,
    Submarine: 3,
    Destroyer: 2
};


function determineShipType(startX, startY, endX, endY) {
    let shipLength;

    if (startX === endX) {
        shipLength = Math.abs(endY - startY) + 1;
    } else if (startY === endY) {
        shipLength = Math.abs(endX - startX) + 1;
    } else {
        return null;
    }

    for (const shipType in shipLengths) {
        if (shipLengths[shipType] === shipLength) {
            return shipType;
        }
    }

    return null;
}

let currentShipType = "Carrier";

function validateAndPlaceShip(startX, startY, endX, endY, shipType) {
    const length = shipLengths[shipType];
    const isHorizontal = startX === endX;
    const isVertical = startY === endY;

    if (!(isHorizontal || isVertical)) {
        alert("Skepp måste placeras horisontellt eller vertikalt.");
        return false;
    }

    const shipLength = isHorizontal ? Math.abs(endY - startY) + 1 : Math.abs(endX - startX) + 1;
    if (shipLength !== length) {
        alert(`Ogiltig längd. ${shipType} måste vara ${length} celler lång.`);
        return false;
    }

    return true;
}

document.getElementById("placeShipManualBtn").addEventListener("click", async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const gameId = urlParams.get('gameId');

    if (!gameId) {
        alert("Game ID saknas i URL.");
        return;
    }

    const coordinates = document.getElementById("coordinates").value.trim();
    const match = coordinates.match(/^(\d+),(\d+)-(\d+),(\d+)$/);

    if (!match) {
        alert("Felaktigt format. Ange koordinater som: 1,1-1,5");
        return;
    }

    const [_, startX, startY, endX, endY] = match.map(Number);

    const shipType = determineShipType(startX, startY, endX, endY);
    if (!shipType) {
        alert("Ogiltig skeppstyp baserat på dessa koordinater.");
        return;
    }

    if (!validateAndPlaceShip(startX, startY, endX, endY, shipType)) return;

    console.log("Sending to server:", { gameId, startX, startY, endX, endY, shipType });

    const response = await fetch("/Game/PlaceShip", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `gameId=${gameId}&startX=${startX}&startY=${startY}&endX=${endX}&endY=${endY}&shipType=${shipType}`
    });

    const result = await response.json();
    if (result.success) {
        alert(`${shipType} placerades framgångsrikt!`);
        for (let x = startX; x <= endX; x++) {
            for (let y = startY; y <= endY; y++) {
                const cell = document.querySelector(`.board-cell[data-x="${x}"][data-y="${y}"]`);
                if (cell) cell.classList.add("ship");
            }
        }
    } else {
        alert(result.message);
    }
});

connection.on("ShipPlaced", ({ userId, startX, startY, endX, endY, shipType }) => {
    // Anropa updateBoard-funktionen för att uppdatera spelarens bräde
    updateBoard(userId, startX, startY, endX, endY, shipType);
});

function updateBoard(userId, startX, startY, endX, endY, shipType) {
    // Kontrollera om det är spelarens egna bräde (userId är 1 för spelaren)
    const boardId = userId === 1 ? "player-board" : null;
    if (!boardId) return; // Om det inte är spelarens bräde, gör inget

    const board = document.getElementById(boardId);

    // Markera cellerna där skeppet är placerat
    const isHorizontal = startY === endY;
    const length = isHorizontal ? Math.abs(endX - startX) + 1 : Math.abs(endY - startY) + 1;

    for (let i = 0; i < length; i++) {
        const x = isHorizontal ? startX + i : startX;
        const y = isHorizontal ? startY : startY + i;

        const cell = board.querySelector(`[data-x="${x}"][data-y="${y}"]`);
        if (cell) {
            cell.classList.add("ship"); // Lägg till klassen 'ship' för att markera cellen
        }
    }
}

/*connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
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

/*function updateShotResult(userId, targetX, targetY, hit) {
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
});*/


