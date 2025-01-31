﻿const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

connection.start()
    .then(() => {
        console.log("SignalR-anslutning upprättad!");
    })
    .catch(err => console.error("SignalR-fel:", err));

function initializeBoard(boardId) {
    const board = document.getElementById(boardId);
    for (let i = 1; i < 11; i++) {
        for (let j = 1; j < 11; j++) {
            const cell = document.createElement('div');
            cell.classList.add('board-cell');
            cell.dataset.x = j;
            cell.dataset.y = i;
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
    const isVertical = startX === endX;
    const isHorizontal = startY === endY;

    if (!(isHorizontal || isVertical)) {
        alert("Skepp måste placeras horisontellt eller vertikalt.");
        return false;
    }

    const shipLength = isHorizontal ? Math.abs(endX - startX) + 1 : Math.abs(endY - startY) + 1;
    if (shipLength !== length) {
        alert(`Ogiltig längd. ${shipType} måste vara ${length} celler lång.`);
        return false;
    }

    return true;
}

document.getElementById("placeShipBtn").addEventListener("click", async () => {
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
    updateBoard(userId, startX, startY, endX, endY, shipType);
});

//----------------------------------------------------------------------------------------------------------------------------------------------------
document.getElementById("fireShotBtn").addEventListener("click", async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const gameId = urlParams.get('gameId');

    if (!gameId) {
        alert("Game ID saknas i URL.");
        return;
    }

    const coordinates = document.getElementById("coordinatesShot").value.trim();
    const match = coordinates.match(/^(\d+),(\d+)$/);

    if (!match) {
        alert("Felaktigt format. Ange koordinater som: 1,3");
        return;
    }

    const [_, targetX, targetY] = match;
    const x = Number(targetX);
    const y = Number(targetY);

    if ((x < 0 && x > 10) || (y < 0 && y > 10)) {
        alert("Ogiltiga koordinater. Ange positiva värden.");
        return;
    }

    console.log(`TargetX: ${x}, TargetY: ${y}`);


    const response = await fetch("/Game/FireShot", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `gameId=${gameId}&targetX=${targetX}&targetY=${targetY}`
    });

    const result = await response.json();
    console.log(result);
    if (result.success) {
        let i = 0;
        alert("Skott avfyrat!");
        const boardId = "opponent-board";
        const board = document.getElementById(boardId);

        const cell = board.querySelector(`[data-x="${targetX}"][data-y="${targetY}"]`);
        if (cell) {
            console.log(`Cell updated with class: ${result.hit ? "hit" : "miss"}`);
            cell.classList.add(result.hit ? "hit" : "miss");
        }

        if (result.gameOver) {
            alert("Spelet är över!");
        }
    } else {
        alert(result.message);
    }
});


connection.on("ShotFired", ({ userId, targetX, targetY, hit, gameOver }) => {
    console.log("ShotFired event received:", { userId, targetX, targetY, hit, gameOver });
});

