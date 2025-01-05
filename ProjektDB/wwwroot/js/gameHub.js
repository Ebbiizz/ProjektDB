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

function joinGame(gameId) {
    connection.invoke("JoinGame", gameId)
        .then(() => {
            console.log(`Gick med i spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid att gå med i spelet:", err));
        //annan UI-förändring?
}
function leaveGame(gameId) {
    connection.invoke("LeaveGame", gameId)
        .then(() => {
            console.log(`Lämnade spelet med ID ${gameId}`);
        })
        .catch(err => console.error("Fel vid att lämna spelet:", err));
        //annan UI-förändring?
}

//allt som finns i hubben måste synkas med js för att göra visuella förändringar/bekräftelser eller kunna samarbeta med spelaren.
