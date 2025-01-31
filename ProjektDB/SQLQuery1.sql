﻿CREATE TABLE Users (
    UserID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL
);

CREATE TABLE Games (
    GameID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Player1ID INT NOT NULL,
    Player2ID INT,
    CurrentTurn INT,
    CreatedAt DATETIME NOT NULL,
    Status NVARCHAR NOT NULL, 
    WinnerID INT,
    FOREIGN KEY (Player1ID) REFERENCES Users(UserID),
    FOREIGN KEY (Player2ID) REFERENCES Users(UserID),
    FOREIGN KEY (WinnerID) REFERENCES Users(UserID)
);

CREATE TABLE Boards (
    BoardID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    GameID INT NOT NULL,
    UserID INT NOT NULL,
    SizeX INT NOT NULL,
    SizeY INT NOT NULL,
    FOREIGN KEY (GameID) REFERENCES Games(GameID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) 
);

CREATE TABLE Ships (
    ShipID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    BoardID INT NOT NULL,
    Type NVARCHAR NOT NULL,
    StartX INT NOT NULL,
    StartY INT NOT NULL,
    EndX INT NOT NULL,
    EndY INT NOT NULL,
    FOREIGN KEY (BoardID) REFERENCES Boards(BoardID) ON DELETE CASCADE
);

CREATE TABLE Shots (
    ShotID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    GameID INT NOT NULL,
    ShooterID INT NOT NULL,
    TargetX INT NOT NULL,
    TargetY INT NOT NULL,
    Hit BIT NOT NULL,
    ShotTime DATETIME NOT NULL,
    FOREIGN KEY (GameID) REFERENCES Games(GameID),
    FOREIGN KEY (ShooterID) REFERENCES Users(UserID)
);

DROP TABLE Users;
DROP TABLE Shots;
DROP TABLE Ships;
DROP TABLE Boards;
DROP TABLE Games;
DROP TABLE Users;
DELETE FROM Users WHERE UserID=3;

ALTER TABLE Ships ALTER COLUMN Type NVARCHAR(10);

DELETE FROM Boards;
DELETE FROM Games;
DELETE FROM Shots;

SELECT TOP 1 GameId
FROM Games
ORDER BY CreatedAt DESC;

ALTER TABLE Games ALTER COLUMN CreatedAt DATETIME;
ALTER TABLE Shots ALTER COLUMN ShotTime DATETIME;

SELECT * FROM Boards WHERE GameId = 51 AND UserId = 1


SELECT * FROM Shots
SELECT * FROM Boards